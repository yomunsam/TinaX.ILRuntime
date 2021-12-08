namespace TinaX.XILRuntime.Redirects
{
    /// <summary>
    /// TinaX ILRuntime 模块 内部 CLR 重定向 注册
    /// </summary>
    public class XILRedirectRegisters
    {
        /// <summary>
        /// 注册CLR方法重定向
        /// </summary>
        /// <param name="xil"></param>
        public static void RegisterCLRMethodRedirections(IXILRuntime xil)
        {
            RedirectXIL.Register(xil); //内置CLR绑定

            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(xil.ILRuntimeAppDomain);
        }
    }
}
