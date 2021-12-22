using System;
using System.ComponentModel;
using TinaX.Container;
using TinaX.Options;
using TinaX.XILRuntime.Options;
using TinaX.XILRuntime.Registers;

namespace TinaX.XILRuntime.Builders
{
    public class XILRuntimeBuilder
    {
        private readonly RegisterManager m_Registers = new RegisterManager();

        public XILRuntimeBuilder(IServiceContainer services)
            => Services = services ?? throw new ArgumentNullException(nameof(services));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly IServiceContainer Services;




        public XILRuntimeBuilder Configure(Action<XILRuntimeOptions> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.Services.AddOptions();
            this.Services.Configure<XILRuntimeOptions>(configuration);
            return this;
        }

        public XILRuntimeBuilder Register(Action<IXILRuntime> registerAction)
        {
            m_Registers.Add(registerAction);
            return this;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public RegisterManager GetRegisterManager() => m_Registers;

    }
}
