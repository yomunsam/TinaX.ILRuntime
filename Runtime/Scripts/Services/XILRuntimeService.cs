using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using ILRuntime.Mono.Cecil.Pdb;
using TinaX.Exceptions;
using TinaX.Options;
using TinaX.Services;
using TinaX.Services.ConfigAssets;
using TinaX.XILRuntime.ConfigAssets;
using TinaX.XILRuntime.Consts;
using TinaX.XILRuntime.Internal;
using TinaX.XILRuntime.Loader;
using TinaX.XILRuntime.Options;
using TinaX.XILRuntime.Structs;
using UnityEngine;
using ILAppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace TinaX.XILRuntime
{
    public class XILRuntimeService : IXILRuntime, IXILRuntimeInternal
    {
        /// <summary>
        /// ILRuntime自动生成的CLR绑定代码 的类名
        /// </summary>
        private const string c_CLRBind_GenCode_TypeName = "ILRuntime.Runtime.Generated.CLRBindings";
        /// <summary>
        /// ILRuntime自动生成的CLR绑定代码 的方法名
        /// </summary>
        private const string c_CLRBind_GenCode_MethodName = "Initialize";


        private readonly XILRuntimeOptions m_Options;
        private readonly IConfigAssetService m_ConfigAssetService;
        private readonly IXCore m_Xcore;
        private readonly ILAppDomain m_AppDomain;

        private readonly Dictionary<string, Stream> m_LoadedAssemblies = new Dictionary<string, Stream>();
        private readonly Dictionary<string, Stream> m_LoadedSymbols = new Dictionary<string, Stream>();

        public XILRuntimeService(IOptions<XILRuntimeOptions> options,
            IConfigAssetService configAssetService,
            IXCore xCore)
        {
            this.m_Options = options.Value;
            this.m_ConfigAssetService = configAssetService;
            this.m_Xcore = xCore;
            this.m_AppDomain = new ILAppDomain(m_Options.DefaultJitFlags);

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            m_AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        }

        private XILRuntimeConfigAsset m_ConfigAsset;
        private bool m_Initialized;
        private IAssemblyLoader m_AssemblyLoader;


        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            if (m_Initialized)
                return;
            if (!m_Options.Enable)
                return;

            //加载配置资产
            m_ConfigAsset = await LoadConfigAssetAsync(m_Options.ConfigAssetLoadPath, cancellationToken);
            if(m_ConfigAsset == null)
            {
                throw new XException($"Failed to load configuration assets \"{m_Options.ConfigAssetLoadPath}\" ");
            }

            //注册CLR重定向
            RegisterCLRMethodRedirections(m_AppDomain);

            //注册委托适配器

            //注册跨域适配器

            //注册CLR绑定
            RegisterGeneratorCLRBindingCode();

            //Assembly Loader
            if(m_ConfigAsset.AssemblyLoaderAsset != null)
            {
                m_AssemblyLoader = m_ConfigAsset.AssemblyLoaderAsset.CreateAssemblyLoader(m_Xcore);
            }
            else
            {
                //Use default loader
                var assetService = m_Xcore.Services.Get<IAssetService>();
                m_AssemblyLoader = new AssetServiceLoader(assetService);
            }

            //加载Assembly
            await LoadAssembliesAsync(m_ConfigAsset.LoadAssemblies);

            m_Initialized = true;
        }


        private UniTask<XILRuntimeConfigAsset> LoadConfigAssetAsync(string loadPath, CancellationToken cancellationToken)
        {
            return m_ConfigAssetService.GetConfigAsync<XILRuntimeConfigAsset>(loadPath, cancellationToken);
        }

        /// <summary>
        /// 内部方法：加载Assembly们到ILRuntime AppDomain
        /// </summary>
        /// <param name="loadInfos"></param>
        /// <returns></returns>
        private async UniTask LoadAssembliesAsync(IEnumerable<AssemblyLoadInfo> loadInfos)
        {
            void handleResult(ref AssemblyLoadResult result)
            {
                if (m_LoadedAssemblies.TryGetValue(result.AssemblyPath, out var _assembly))
                {
                    _assembly.Dispose();
                    m_LoadedAssemblies.Remove(result.AssemblyPath);
                }
                if (m_LoadedSymbols.TryGetValue(result.SymbolPath, out var _symbol))
                {
                    _symbol.Dispose();
                    m_LoadedSymbols.Remove(result.SymbolPath);
                }


                if (result.SymbolStream != null)
                {
                    m_LoadedSymbols.Add(result.SymbolPath, result.SymbolStream);
                }
                if (result.AssemblyStream != null)
                {
                    m_LoadedAssemblies.Add(result.AssemblyPath, result.AssemblyStream);
                    if (result.SymbolStream != null)
                    {
                        m_AppDomain.LoadAssembly(result.AssemblyStream, result.SymbolStream, new PdbReaderProvider());
                    }
                    else
                    {
                        m_AppDomain.LoadAssembly(result.AssemblyStream);
                    }
                }
            }

            var loadInfo_Enumerator = loadInfos.GetEnumerator();

            if (m_AssemblyLoader.SupportAsynchronous)
            {
                //异步加载逻辑
                List<UniTask<AssemblyLoadResult>> loadTasks = new List<UniTask<AssemblyLoadResult>>();
                while (loadInfo_Enumerator.MoveNext())
                {
                    if (loadInfo_Enumerator.Current.AssemblyPath.IsNullOrEmpty())
                    {
                        if(!loadInfo_Enumerator.Current.SymbolPath.IsNullOrEmpty())
                        {
                            Debug.LogWarningFormat("[{0}]Invalid \"Load Assemblies\" configuration item --> Assembly Path: {1}, Symbol Path:{2}", XILConsts.ModuleName, loadInfo_Enumerator.Current.AssemblyPath, loadInfo_Enumerator.Current.SymbolPath);
                        }
                        continue;
                    }
                    loadTasks.Add(m_AssemblyLoader.LoadAssemblyAsync(loadInfo_Enumerator.Current));
                }
                if(loadTasks.Count > 0)
                {
                    var loadResults = await UniTask.WhenAll(loadTasks);
                    for(int i = 0; i < loadResults.Length; i++)
                    {
                        var result = loadResults[i];
                        handleResult(ref result);
                    }
                }
            }
            else
            {
                //同步加载
                while (loadInfo_Enumerator.MoveNext())
                {
                    if (loadInfo_Enumerator.Current.AssemblyPath.IsNullOrEmpty())
                    {
                        if (!loadInfo_Enumerator.Current.SymbolPath.IsNullOrEmpty())
                        {
                            Debug.LogWarningFormat("[{0}]Invalid \"Load Assemblies\" configuration item --> Assembly Path: {1}, Symbol Path:{2}", XILConsts.ModuleName, loadInfo_Enumerator.Current.AssemblyPath, loadInfo_Enumerator.Current.SymbolPath);
                        }
                        continue;
                    }
                    var loadResult = m_AssemblyLoader.LoadAssembly(loadInfo_Enumerator.Current);
                    handleResult(ref loadResult);
                }
            }

        }

        /// <summary>
        /// 注册CLR方法重定向
        /// </summary>
        private void RegisterCLRMethodRedirections(ILAppDomain appDomain)
        {

        }

        /// <summary>
        /// 内部方法：注册生成出来的CLR绑定代码
        /// </summary>
        private void RegisterGeneratorCLRBindingCode()
        {
            /*
             * 我们希望在framework里完成CLR绑定代码的自动注册
             * 1. 这段生成代码不一定存在
             * 2. XILRuntime无法确定这个代码会存在于哪个Assembly里
             * 
             * 所以，使用反射的方式进行调用
             */
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            Type type = null;
            foreach (var asm in assemblys)
            {
                type = asm.GetType(c_CLRBind_GenCode_TypeName);
                if (type != null)
                    break;
            }

            if (type != null)
            {
                var method = type.GetMethod(c_CLRBind_GenCode_MethodName, new Type[] { typeof(ILAppDomain) });
                if (method != null)
                {
                    method.Invoke(null, new object[] { m_AppDomain });
#if TINAX_DEV
                    Debug.Log("<color=green>已执行CLR生成代码的注册</color>");
#endif
                }
                else
                    Debug.LogError($"[{XILConsts.ModuleName}] CLR binding failed. Method \"Initialize\" not found");
            }
            else
                Debug.LogWarning($"[{XILConsts.ModuleName}] CLR binding failed. Generated code not found");
        }

    }
}
