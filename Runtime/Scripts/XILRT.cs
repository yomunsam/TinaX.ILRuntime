using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TinaX.XILRuntime.Const;
using TinaX.XILRuntime.Exceptions;
using TinaX.XILRuntime.Internal;
using TinaX.Services;
using UnityEngine;
using ILAppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using ILRuntime.Mono.Cecil.Pdb;
using System;
using ILRuntime.Runtime.Enviorment;
using System.Reflection;
using ILRuntime.Runtime;
using ILRuntime.CLR.TypeSystem;
using TinaX.Container;
using ILRuntime.Reflection;
using TinaX.XILRuntime.Utils;

namespace TinaX.XILRuntime
{
    public class XILRT : IXILRuntime , IXILRTInternal , IAppDomain
    {
        /// <summary>
        /// 是否要加载调试符号（默认以TinaX的开发模式来决定
        /// </summary>
        public bool LoadSymbol { get; set; }
        public DelegateManager DelegateManager => m_AppDomain?.DelegateManager;

        private IAssetService m_Assets;
        private bool m_Inited = false;
        private XILRTConfig m_Config;
        private ILAppDomain m_AppDomain;
        private IXCore m_Core;
        private IServiceContainer m_XServices;

        private Dictionary<string, Stream> m_LoadedAssemblies = new Dictionary<string, Stream>();
        private Dictionary<string, Stream> m_LoadedSymbols = new Dictionary<string, Stream>();

        public XILRT(IAssetService assets)
        {
            m_Assets = assets;
            m_AppDomain = new ILAppDomain();
            m_Core = XCore.GetMainInstance();
            m_XServices = m_Core.Services;

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            m_AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            LoadSymbol = XCore.GetMainInstance().DevelopMode;
#if UNITY_EDITOR
            LoadSymbol = true;
#endif
        }

        /*
         * 启动顺序：
         * - 注册CLR重定向
         * - 注册委托适配器
         * - 注册CLR绑定
         * - 加载Assembly和它的小伙伴们
         * 
         */
        public async Task<XException> StartAsync()
        {
            if (m_Inited) return null;
            m_Config = XConfig.GetConfig<XILRTConfig>(XILConst.ConfigPath_Resources);
            if (m_Config == null)
                return new XILRTException("Config file not found :" + XILConst.ConfigPath_Resources);

            if (!m_Config.Enable) return null;

            //注册CLR重定向
            registerCLRMethodRedirections();

            //注册委托适配器
            registerDelegates();

            //加载Assemblies
            var e_load = await this.loadAssemblies(m_Config.LoadAssemblies);
            if (e_load != null)
                return e_load;


            m_Inited = true;
            return null;
        }

        /// <summary>
        /// 注册 跨域继承
        /// </summary>
        /// <param name="adaptor"></param>
        /// <returns></returns>
        public IXILRuntime RegisterCrossBindingAdaptor(CrossBindingAdaptor adaptor)
        {
            m_AppDomain?.RegisterCrossBindingAdaptor(adaptor);
            return this;
        }

        /// <summary>
        /// 注册 CLR重定向
        /// </summary>
        /// <param name="method"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public IXILRuntime RegisterCLRMethodRedirection(MethodBase method,CLRRedirectionDelegate func)
        {
            m_AppDomain?.RegisterCLRMethodRedirection(method, func);
            return this;
        }

        public object CreateInstance(Type type, params object[] args)
        {
            if (type is ILRuntimeType || type is ILRuntimeWrapperType)
                return m_AppDomain.Instantiate(type.FullName, args);
            else
                return Activator.CreateInstance(type, args);
        }

        public void InjectObject(object target)
        {
            BindingFlags _bindFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            Type actualType = target.GetActualType();

            foreach (var field in actualType.GetFields(_bindFlag))
            {
                string service_name = XILUtil.GetCatLibServiceName(field);

                var attr = field.GetCustomAttribute<InjectAttribute>(true);
                if (attr == null)
                    continue;
                if (m_XServices.TryGet(service_name, out var _service))
                {
                    field.SetValue(target, _service);
                    continue;
                }

                if (attr.Nullable)
                    continue;
                else
                    throw new ServiceNotFoundException(field.FieldType); //抛异常
            }

            foreach (var property in actualType.GetProperties(_bindFlag))
            {
                string service_name = XILUtil.GetCatLibServiceName(property);
                var attr = property.GetCustomAttribute<InjectAttribute>(true);
                if (attr == null)
                    continue;
                if (m_XServices.TryGet(service_name, out var _service))
                {
                    property.SetValue(target, _service);
                    continue;
                }

                if (attr.Nullable)
                    continue;
                else
                    throw new ServiceNotFoundException(property.PropertyType); //抛异常
            }
        }

        public void InvokeEntryMethod()
        {
            if (m_Config == null || !m_Config.Enable)
                return;
            if(m_Config.EntryClass.IsNullOrEmpty()
                || m_Config.EntryMethod.IsNullOrEmpty()
                || m_Config.EntryClass.IsNullOrWhiteSpace()
                || m_Config.EntryMethod.IsNullOrWhiteSpace())
            {
                Debug.LogWarning($"[{Const.XILConst.ServiceName}] Invalid entry class name or method name");
                return;
            }
            m_AppDomain.Invoke(m_Config.EntryClass, m_Config.EntryMethod, null, null);
        }

        public bool TryGetServiceName(Type type, out string name)
        {
            name = type.FullName;
            return type is ILRuntimeType || type is ILRuntimeWrapperType;
        }

        private async Task<XException> loadAssemblies(IEnumerable<AssemblyAndSymbolPath> paths)
        {
            foreach(var path in paths)
            {
                try
                {
                    await this.loadAssembly(path);
                }
                catch(XException e)
                {
                    return e;
                }
            }

            return null;
        }

        private async Task loadAssembly(AssemblyAndSymbolPath path)
        {
            if (m_LoadedAssemblies.TryGetValue(path.AssemblyPath, out var _assembly))
            {
                _assembly.Dispose();
                m_LoadedAssemblies.Remove(path.AssemblyPath);
            }
            if(m_LoadedSymbols.TryGetValue(path.SymbolPath, out var _symbol))
            {
                _symbol.Dispose();
                m_LoadedSymbols.Remove(path.SymbolPath);
            }

            var assembly_textasset = await m_Assets.LoadAsync<TextAsset>(path.AssemblyPath);
            var assembly_stream = new MemoryStream(assembly_textasset.bytes);
            m_Assets.Release(assembly_textasset);

            MemoryStream symbol_stream = null;
            if (this.LoadSymbol)
            {
                try
                {
                    var symbol_textasset = await m_Assets.LoadAsync<TextAsset>(path.SymbolPath);
                    symbol_stream = new MemoryStream(symbol_textasset.bytes);
                    m_Assets.Release(symbol_textasset);
                }
                catch { }
            }

            m_LoadedAssemblies.Add(path.AssemblyPath, assembly_stream);
            if (this.LoadSymbol)
            {
                if (symbol_stream != null)
                    m_LoadedSymbols.Add(path.SymbolPath, symbol_stream);
                else
                    Debug.LogWarning($"[{XILConst.ServiceName}] Load symbol file failed: {path.SymbolPath}");
            }


            if (this.LoadSymbol && symbol_stream != null)
                m_AppDomain.LoadAssembly(assembly_stream, symbol_stream, new PdbReaderProvider());
            else
                m_AppDomain.LoadAssembly(assembly_stream);
        } 

        private void registerCLRMethodRedirections()
        {
            InternalRegisters.RegisterCLRMethodRedirections(this);
        }

        /// <summary>
        /// 内部方法：注册委托适配器
        /// </summary>
        private void registerDelegates()
        {
            //内部委托适配器
            InternalRegisters.RegisterDelegates(this);
        }

        
    }
}

