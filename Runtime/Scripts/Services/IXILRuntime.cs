using ILRuntime.Runtime.Enviorment;

namespace TinaX.XILRuntime
{
    public interface IXILRuntime
    {
        /// <summary>
        /// 注册 跨域适配器
        /// </summary>
        /// <param name="adaptor"></param>
        /// <returns></returns>
        IXILRuntime RegisterCrossBindingAdaptor(CrossBindingAdaptor adaptor);
    }
}
