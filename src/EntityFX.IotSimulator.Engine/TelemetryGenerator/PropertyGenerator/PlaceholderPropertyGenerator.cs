using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator
{
    public class PlaceholderPropertyGenerator : PropertyGenerator<string, Enum>
    {
        public IEnumerable<IPropertyGenerator> PropertyGenerators { get; set; }

        public string Template { get; set; }

        private readonly Regex placeholderRegex;

        private const string PlaceholderRegexString = @"{{\s*(\w+)\s*}}";

        public PlaceholderPropertyGenerator(string name, IEnumerable<IPropertyGenerator> propertyGenerators, string template, Dictionary<string, object> variables) : 
            base(name, null, variables)
        {
            PropertyGenerators = propertyGenerators;
            Template = template;
            placeholderRegex = new Regex(PlaceholderRegexString);
        }

        public override string Value
        {
            get
            {
                var dict = PropertyGenerators.ToDictionary(p => p.Name, p => p.Format == null ? p.Value?.ToString() : string.Format(CultureInfo.InvariantCulture, p.Format, p.Value ?? string.Empty));

                var formatted = placeholderRegex.Replace(Template, m => {
                    var placeholderName = m.Groups[1].Value;
                    if (!dict.ContainsKey(placeholderName))
                    {
                        return string.Empty;
                    }

                    return dict[placeholderName]; 
                });

                return formatted;
            }
        }
    }
}