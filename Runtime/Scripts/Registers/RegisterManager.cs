using System;
using System.Collections.Generic;

namespace TinaX.XILRuntime.Registers
{
    /// <summary>
    /// 注册管理器
    /// </summary>
    public class RegisterManager
    {
        private readonly List<Action<IXILRuntime>> m_Registers;

        public RegisterManager()
        {
            m_Registers = new List<Action<IXILRuntime>>();
        }

        public RegisterManager(IEnumerable<Action<IXILRuntime>> collection)
        {
            m_Registers = new List<Action<IXILRuntime>>(collection);
        }

        public void Add(Action<IXILRuntime> register)
        {
            m_Registers.Add(register);
        }

        public int Count => m_Registers.Count;

        public List<Action<IXILRuntime>> GetAll() => m_Registers;

    }
}
