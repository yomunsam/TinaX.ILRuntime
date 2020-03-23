using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX;
using TinaX.Services;
using System.Threading.Tasks;
using TinaX.XILRuntime.Const;
using TinaX.XILRuntime.Internal;

namespace TinaX.XILRuntime
{
    [XServiceProviderOrder(100)]
    public class XRuntimeProvider : IXServiceProvider
    {
        public string ServiceName => XRuntimeConst.ServiceName;

        public Task<bool> OnInit() => Task.FromResult(true);

        public XException GetInitException() => null;

        /// <summary>
        /// 服务注册
        /// </summary>
        public void OnServiceRegister()
        {
            XCore.GetMainInstance().BindSingletonService<IXRuntime, IAppDomain, XRuntime>().SetAlias<IXRuntimeInternal>();
        }

        /// <summary>
        /// 服务启动
        /// </summary>
        /// <returns></returns>
        public Task<bool> OnStart()
        {
            return XCore.GetMainInstance().GetService<IXRuntimeInternal>().Start();
        }

        public XException GetStartException()
        {
            return XCore.GetMainInstance().GetService<IXRuntimeInternal>().GetStartException();
        }

        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;

        

        
    }
}

