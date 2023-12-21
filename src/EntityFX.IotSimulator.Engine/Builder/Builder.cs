using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.Builder
{
    public class Builder<TType, TStubBuilderType> : BuilderBase<TType>, IBuilder<TType>
        where TStubBuilderType : IBuilder<TType>
    {
        private readonly string assemblyName;
        private readonly string senderTypeName;

        public Builder(ILogger logger, IConfiguration configuration, Dictionary<string, object> extraSettings,
            string assemblyName, string senderTypeName)
            : base(logger, configuration, extraSettings)
        {
            this.assemblyName = assemblyName;
            this.senderTypeName = senderTypeName;
        }

        public override TType Build()
        {
            var builder = GetBuilder();
            return builder.Build();
        }

        protected virtual IBuilder<TType> GetBuilder()
        {
            var builder = this.GetBuilder<TType, TStubBuilderType>(
                assemblyName, senderTypeName, logger, configuration, extraSettings);
            return builder;
        }
    }
}
