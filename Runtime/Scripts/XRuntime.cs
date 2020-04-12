using ILRuntime.Mono.Cecil.Cil;
using ILRuntime.Mono.Cecil.Pdb;
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TinaX.XILRuntime.Const;
using TinaX.XILRuntime.Exceptions;
using TinaX.XILRuntime.Internal;
using UnityEngine;
using AppDomain = System.AppDomain;
using ILAppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace TinaX.XILRuntime
{
    public class XRuntime : IXRuntime, IXRuntimeInternal , TinaX.Services.IAppDomain
    {
        private ILAppDomain mAppDomain;
        private XRuntimeConfig mConfig;
        private bool mInited = false;

        private Stream mAssemblyStream;
        private Stream mSymbolStream;

        private string mAssemblyFilePath;
        private string mSymbolFilePath;

        private IXCore mCore;

        private XRTException mStartException;

        private string mEntryMethod_Type;
        private string mEntryMethod_Method;

        public ILAppDomain ILAppDomain => this.mAppDomain;

        public XRuntime()
        {
            mCore = XCore.GetMainInstance();
            mAssemblyFilePath = Path.Combine(XCore.LocalStorage_TinaX, "xruntime", XRuntimeConst.AssemblyFileName);
            mSymbolFilePath = Path.Combine(XCore.LocalStorage_TinaX, "runtime", XRuntimeConst.SymbolFileName);
            mAppDomain = new ILAppDomain();
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            mAppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            registerCLR();
        }

        public async Task<bool> Start()
        {
            if (mInited) return true;
            mConfig = XConfig.GetConfig<XRuntimeConfig>(XRuntimeConst.ConfigPath_Resources, AssetLoadType.Resources, false);
            if (mConfig == null)
            {
                mStartException = new XRTException("[TinaX.ILRuntime] Connot found config file.", XRuntimeErrorCode.ConfigFileInvalid); ;
                return false;
            }

            #region config
            if (!mConfig.EnableILRuntime) return true;
            
            if(mConfig.EntryMethod.IsNullOrEmpty())
            {
                mStartException = new XRTException("[TinaX.ILRuntime] Entry method is empty", XRuntimeErrorCode.InvalidEntryMethod);
                return false;
            }
            int _method_last_index = mConfig.EntryMethod.LastIndexOf('.');
            if(_method_last_index <= 0)
            {
                mStartException = new XRTException("[TinaX.ILRuntime] Entry method is invalid:" + mConfig.EntryMethod, XRuntimeErrorCode.InvalidEntryMethod);
                return false;
            }
            mEntryMethod_Type = mConfig.EntryMethod.Substring(0, _method_last_index);
            mEntryMethod_Method = mConfig.EntryMethod.Substring(_method_last_index + 1, mConfig.EntryMethod.Length - _method_last_index - 1);

            #endregion

            bool success = await TryInitILAppDomain();

            if (success)
            {
                //cli binding
                string type_name = "ILRuntime.Runtime.Generated.CLRBindings";
                string method_name = "Initialize";
                var assemblys = AppDomain.CurrentDomain.GetAssemblies();
                Type type = null;
                foreach(var asm in assemblys)
                {
                    type = asm.GetType(type_name);
                    if (type != null)
                        break;
                }
                if (type != null)
                {
                    var method = type.GetMethod(method_name, new Type[] { typeof(ILAppDomain) });
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { mAppDomain });
                    }
                    else
                        Debug.LogError("[TinaX.ILRuntime] CLR binding failed. the method \"Initialize\" not found");
                }
                else
                    Debug.LogWarning("[TinaX.ILRuntime] CLRBindings code not found. Please generate CLR binding code in Menu or ProjectSettings.");
            }

            mInited = true;
            return true;
        }
        public XRTException GetStartException() { return mStartException; }

        public void OnQuit()
        {
            if (mAssemblyStream != null)
                mAssemblyStream.Dispose();
            mAssemblyStream = null;
            if (mSymbolStream != null)
                mSymbolStream.Dispose();
            mSymbolStream = null;
        }

        public object Invoke(string type, string method, object instance = null, params object[] param)
        {
            if (!mInited)
                throw new XException("[TinaX.ILRuntime] you cannot invoke \"LoadAssembly\" before service started.");

            return this.InvokeILRT(type, method, instance, param);
        }

        public void RegisterCLRMethodRedirection(MethodBase method, CLRRedirectionDelegate func)
        {
            if(mAppDomain != null)
            {
                mAppDomain.RegisterCLRMethodRedirection(method, func);
            }
        }

        public void RegisterCLRMethodRedirection(IEnumerable<CLRRedirectionInfo> funcs)
        {
            foreach(var item in funcs)
            {
                mAppDomain?.RegisterCLRMethodRedirection(item.method, item.func);
            }
        }

        public void RegisterCrossBindingAdaptor(CrossBindingAdaptor adaptor)
        {
            if(mAppDomain != null)
            {
                mAppDomain.RegisterCrossBindingAdaptor(adaptor);
            }
        }

        public object CreateInstance(Type type, params object[] args)
        {
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                var obj = mAppDomain?.Instantiate(type.FullName, args);
                if (obj is CrossBindingAdaptorType)
                    return ((CrossBindingAdaptorType)obj).ILInstance;
                else
                    return obj;
            }
            else
                return Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// 调用入口方法，由XRuntime的Bootstarp调用
        /// </summary>
        public void InvokeEntryMathod()
        {
            if (mInited)
            {
                this.InvokeILRT(mEntryMethod_Type, mEntryMethod_Method);
            }
        }


        private async Task<bool> TryInitILAppDomain()
        {
            Stream dll_stream = null;
            Stream pdb_stream = null;

            bool load_assembly_by_frameowrk = (mConfig.AssemblyLoadMode == AssemblyLoadingMethod.FrameworkAssetsManager);

            if (load_assembly_by_frameowrk)
            {
                if (mCore.TryGetBuiltinService<TinaX.Services.IAssetService>(out var assets))
                {
                    try
                    {
                        TextAsset dll_ta = await assets.LoadAsync<TextAsset>(mConfig.Dll_LoadPathByFrameworkAssetsManager);
                        dll_stream = new MemoryStream(dll_ta.bytes);
                    }
                    catch (XException e) 
                    {
                        Debug.LogError("[TinaX.ILRuntime] load assembly failed:" + e.Message);
                    }

                    if (dll_stream != null && mCore.DevelopMode)
                    {
                        try
                        {
                            TextAsset pdb_ta = await assets.LoadAsync<TextAsset>(mConfig.Symbol_LoadPathByFrameworkAssetsManager);
                            pdb_stream = new MemoryStream(pdb_ta.bytes);
                        }
                        catch (XException) { }
                    }
                }
                else
                {
                    throw new XRTException("[TinaX.ILRuntime] Load Assembly Failed: Framework not implementationed assets manager interface.", XRuntimeErrorCode.FrameworkAssetsManagerInterfaceInValid);
                }
            }
            else
            {
                if (File.Exists(mAssemblyFilePath))
                {
                    dll_stream = new FileStream(mAssemblyFilePath, FileMode.Open);
                    if (mSymbolFilePath == null)
                    {
                        pdb_stream = new FileStream(mSymbolFilePath, FileMode.Open);
                    }
                }
            }

            if (dll_stream != null)
            {
                this.loadAssembly(dll_stream, pdb_stream);
                return true;
            }
            else
            {
                return false;
            }

        }

        private void loadAssembly(Stream assembly, Stream symbol = null, ISymbolReaderProvider symbolReaderProvider = null)
        {
            
            if (mAssemblyStream != null) mAssemblyStream.Dispose();
            if (mSymbolStream != null) mSymbolStream.Dispose();
            mAssemblyStream = assembly;
            mSymbolStream = symbol;

            if (symbol == null)
                mAppDomain.LoadAssembly(mAssemblyStream);
            else
            {
                if (symbolReaderProvider == null)
                    mAppDomain.LoadAssembly(mAssemblyStream, mSymbolStream, new PdbReaderProvider());
                else
                    mAppDomain.LoadAssembly(mAssemblyStream, mSymbolStream, symbolReaderProvider);
            }
        }

        

        
        private object InvokeILRT(string type, string method, object instance = null, params object[] param)
        {
            return mAppDomain.Invoke(type, method, instance, param);
        }

        private unsafe void registerCLR()
        {
            //test
            List<CLRRedirectionInfo> refirections = new List<CLRRedirectionInfo>(CLRRedirectionDefineInternal.Redirections);

            //要用反射来获取开发者定义的列表嘛？待决定

            foreach (var item in refirections)
                mAppDomain.RegisterCLRMethodRedirection(item.method, item.func);
        }

        private void print(object obj)
        {
#if TINAX_DEBUG_DEV
            Debug.Log(obj);
#endif
        }

        

        private static class XRuntimeI18N
        {
            private static bool? _isChinese;
            private static bool IsChinese
            {
                get
                {
                    if (_isChinese == null)
                    {
                        _isChinese = (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified);
                    }
                    return _isChinese.Value;
                }
            }

            private static bool? _nihongo_desuka;
            private static bool NihongoDesuka
            {
                get
                {
                    if (_nihongo_desuka == null)
                        _nihongo_desuka = (Application.systemLanguage == SystemLanguage.Japanese);
                    return _nihongo_desuka.Value;
                }
            }

            public static string Tips_DisableILRuntimeInEditor
            {
                get
                {
                    if (IsChinese)
                        return $"[TinaX.ILRuntime] <color=#{TinaX.Internal.XEditorColorDefine.Color_Emphasize_16}>根据编辑器策略，不使用ILRuntime加载Assembly.</color>";
                    return $"[TinaX.ILRuntime] <color=#{TinaX.Internal.XEditorColorDefine.Color_Emphasize_16}>According to the editor policy, not use ILRuntime to load the updatable Assembly.</color>";
                }
            }

            public static string Tips_LoadFromLibraryInEditor
            {
                get
                {
                    if (IsChinese)
                        return $"[TinaX.ILRuntime] <color=#{TinaX.Internal.XEditorColorDefine.Color_Emphasize_16}>根据编辑器策略，将强制从项目的\"Library/ScriptAssemblies\"目录中加载Assembly.</color>";
                    if(NihongoDesuka)
                        return $"[TinaX.ILRuntime] <color=#{TinaX.Internal.XEditorColorDefine.Color_Emphasize_16}>エディターポリシーによると、アセンブリはプロジェクトの \"Library/ScriptAssemblies\" ディレクトリから強制的に読み込まれます。</color>";
                    return $"[TinaX.ILRuntime] <color=#{TinaX.Internal.XEditorColorDefine.Color_Emphasize_16}>According to the editor policy, Assembly will be forced to load from the project's \"Library/ScriptAssemblies\" directory.</color>";
                }
            }

        }


    }
}
