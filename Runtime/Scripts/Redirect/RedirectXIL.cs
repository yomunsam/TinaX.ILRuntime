/*
 * 此处代码借鉴自CatLib (MIT License)
 * https://github.com/CatLib/CatLib.ILRuntime/blob/master/src/Redirect
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.XILRuntime.Internal.Redirect
{
    internal static unsafe partial class RedirectXIL
    {
        /// <summary>
        /// 重定向映射表
        /// </summary>
        private static readonly Dictionary<Type, RedirectMapping> m_Mappings;

        static RedirectXIL()
        {
            m_Mappings = new Dictionary<Type, RedirectMapping>();

            Register_IServiceContainer();
            Register_IXCore();
            Register_UnityEngine_Debug();
        }

        /// <summary>
        /// 注册CLR重定向
        /// </summary>
        /// <param name="appDomain">AppDomain</param>
        public static void Register(IXILRuntime xil)
        {
            foreach(var item in m_Mappings)
            {
                var mapping = item.Value;
                var type = item.Key;
                var methods = type.GetMethods();

                foreach (var method in methods)
                {
                    var redirection = mapping.GetRedirection(method);
                    if (redirection == null)
                        continue;

                    xil.RegisterCLRMethodRedirection(method, redirection);
                }
            }
        }


        private static void GetMappingOrCreate(Type type , out RedirectMapping mapping)
        {
            if (m_Mappings.ContainsKey(type))
            {
                mapping = m_Mappings[type];
            }
            else
            {
                mapping = new RedirectMapping();
                m_Mappings.Add(type, mapping);
            }
        }

    }
}
