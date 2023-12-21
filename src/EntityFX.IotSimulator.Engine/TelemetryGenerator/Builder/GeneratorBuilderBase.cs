using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using System;
using System.Collections.Generic;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder
{
    public class GeneratorBuilderBase<TTypeEnum, TValueType, TPropertyGenerator, TBuilder>
        where TTypeEnum : Enum
        where TPropertyGenerator : PropertyGenerator<TValueType, TTypeEnum>, IPropertyGenerator<TValueType, TTypeEnum>
        where TBuilder : GeneratorBuilderBase<TTypeEnum, TValueType, TPropertyGenerator, TBuilder>
    {
        protected TTypeEnum type;

        protected Dictionary<string, object> variables;

        protected TValueType constantValue;

        protected string name;

        public TBuilder WithType(TTypeEnum type)
        {
            this.type = type;

            return (TBuilder)this;
        }

        public TBuilder WithVariables(Dictionary<string, object> variables)
        {
            this.variables = variables;

            return (TBuilder)this;
        }

        public TBuilder WithName(string name)
        {
            this.name = name;

            return (TBuilder)this;
        }

        public TBuilder WithConstant(TValueType constant)
        {
            this.constantValue = constant;

            return (TBuilder)this;
        }

        public virtual TBuilder WithDefault()
        {
            return (TBuilder)this;
        }

        public TPropertyGenerator Build()
        {
            Validate();
            return BuildInstance();
        }

        protected virtual TPropertyGenerator BuildInstance()
        {
            return (TPropertyGenerator)Activator.CreateInstance(typeof(TPropertyGenerator), name, type, variables);
        }

        protected virtual void Validate()
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
        }
    }
}