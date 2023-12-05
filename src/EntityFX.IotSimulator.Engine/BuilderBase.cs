using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine
{
    public abstract class BuilderBase<TType> : IBuilder<TType>
    {
        protected readonly ILogger logger;
        protected readonly IConfiguration configuration;
        protected readonly Dictionary<string, object> extraSettings;

        public BuilderBase(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.extraSettings = extraSettings;
        }

        public abstract TType Build();
    }
}
