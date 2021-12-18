using System.Reflection;
using ILRuntime.Runtime.Enviorment;
using TinaX.Core.Activator;
using TinaX.Core.Container;

namespace TinaX.XILRuntime
{
    public interface IXILRuntime
    {
        AppDomain ILRuntimeAppDomain { get; }
        ICreateInstance InsatnceCreator { get; }
        IServiceInjector Serviceinjector { get; }
        DelegateManager DelegateManager { get; }

        /// <summary>
        /// 注册 CLR 方法重定向
        /// </summary>
        /// <param name="method"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        IXILRuntime RegisterCLRMethodRedirection(MethodBase method, CLRRedirectionDelegate func);

        /// <summary>
        /// 注册 跨域适配器
        /// </summary>
        /// <param name="adaptor"></param>
        /// <returns></returns>
        IXILRuntime RegisterCrossBindingAdaptor(CrossBindingAdaptor adaptor);
    }
}
