using EntityFX.IotSimulator.Engine.Settings.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.Builder;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Enums;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator.Sequence;
using EntityFX.IotSimulator.Engine.TelemetryGenerator.PropertyGenerator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace EntityFX.IotSimulator.Common
{
    public class PropertyGeneratorBuilder
    {

        private IPropertyGenerator BuildBoolGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem
            , Dictionary<string, object> variables)
        {
            if (propertyValueItem.Constant != null)
            {
                return new BoolGeneratorBuilder()
                    .WithName(item.Key)
                    .WithConstant(Convert.ToBoolean(propertyValueItem.Constant, CultureInfo.InvariantCulture))
                    .WithDefault()
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Random != null)
            {
                return new BoolGeneratorBuilder()
                    .WithName(item.Key)
                    .WithType(BoolType.Random)
                    .WithNull(propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                    .WithDefault()
                    .WithVariables(variables)
                    .Build();
            }

            return new BoolGeneratorBuilder()
                .WithName(item.Key)
                .WithType(BoolType.Sequece)
                .WithNull(propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                .WithDefault()
                .WithVariables(variables)
                .Build();
        }

        private IPropertyGenerator BuildNumberGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem
            , Dictionary<string, object> variables)
        {
            if (propertyValueItem.Constant != null)
            {
                return new NumberGeneratorBuilder()
                    .WithName(item.Key)
                    .WithConstant(Convert.ToDecimal(propertyValueItem.Constant, CultureInfo.InvariantCulture))
                    .WithRoundDecimals(propertyValueItem.RoundDecimals)
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random != true)
            {
                return new NumberGeneratorBuilder()
                    .WithName(item.Key)
                    .WithNumberSequence(new NumberSequence()
                    {
                        From = Convert.ToDecimal(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                        To = Convert.ToDecimal(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture),
                        Step = propertyValueItem.Sequence.Step != null ?
                                Convert.ToDecimal(propertyValueItem.Sequence.Step, CultureInfo.InvariantCulture) : 1,
                        IsTwoWay = propertyValueItem.IsTwoWay.HasValue ? propertyValueItem.IsTwoWay.Value : false
                    })
                    .WithRoundDecimals(propertyValueItem.RoundDecimals)
                    .WithNull(propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Enum != null)
            {
                return new NumberGeneratorBuilder()
                    .WithName(item.Key)
                    .WithEnumValue(new EnumValues<decimal?>(propertyValueItem.Enum.Select(
                        f => string.IsNullOrEmpty(f) ? null : (decimal?)Convert.ToDecimal(f, CultureInfo.InvariantCulture)
                    ).ToArray()), propertyValueItem.Random ?? false)
                    .WithRoundDecimals(propertyValueItem.RoundDecimals)
                    .WithNull(propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                    .WithTwoWay(propertyValueItem.IsTwoWay ?? false)
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random == true)
            {
                if (propertyValueItem.Sequence?.IsDouble == true)
                {
                    return new NumberGeneratorBuilder()
                        .WithName(item.Key)
                        .WithRandomRange(new RandomRange()
                        {
                            From = Convert.ToDecimal(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                            To = Convert.ToDecimal(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture),
                            IsDouble = true
                        })
                        .WithRoundDecimals(propertyValueItem.RoundDecimals)
                        .WithNull(propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                        .WithVariables(variables)
                        .Build();
                }

                return new NumberGeneratorBuilder()
                    .WithName(item.Key)
                    .WithRandomRange(new RandomRange()
                    {
                        From = Convert.ToInt64(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                        To = Convert.ToInt64(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture),
                        IsDouble = propertyValueItem.Sequence?.IsDouble ?? false
                    })
                    .WithRoundDecimals(propertyValueItem.RoundDecimals)
                    .WithNull(propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                    .WithVariables(variables)
                    .Build();
            }
            else
            {
                return new NumberGeneratorBuilder()
                    .WithName(item.Key)
                    .WithConstant(1)
                    .WithRoundDecimals(propertyValueItem.RoundDecimals)
                    .WithVariables(variables)
                    .Build();
            }
        }

        public IPropertyGenerator BuildPropertyGenerator(KeyValuePair<string, TelemetryPropertySetting> item,
            Dictionary<string, object> variables)
        {
            IPropertyGenerator propertyGenerator = null;
            var propertyValueItem = item.Value;

            if (propertyValueItem.Type == TelemetryPropertyType.Number)
            {
                propertyGenerator = BuildNumberGenerator(item, propertyValueItem, variables);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.String)
            {
                propertyGenerator = BuildStringGenerator(item, propertyValueItem, variables);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.Bool)
            {
                propertyGenerator = BuildBoolGenerator(item, propertyValueItem, variables);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.Timestamp)
            {
                propertyGenerator = BuildTimestampGenerator(item, propertyValueItem, variables);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.DateTime)
            {
                propertyGenerator = BuildDateTimeGenerator(item, propertyValueItem, variables);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.Complex)
            {
                propertyGenerator = new ComplexPropertyGenerator(item.Key,
                    propertyValueItem.Properties.Select(p => BuildPropertyGenerator(p, variables)).ToArray(), variables);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.Placeholder)
            {
                string template = string.Empty;
                if (File.Exists(propertyValueItem.TemplatePath))
                {
                    template = File.ReadAllText(propertyValueItem.TemplatePath);
                }
                propertyGenerator = new PlaceholderPropertyGenerator(item.Key,
                    propertyValueItem.Properties.Select(p => BuildPropertyGenerator(p, variables)).ToArray(), template, variables);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.GeoLocation)
            {
                if (propertyValueItem.GeoConstant != null)
                {
                    propertyGenerator = new GeoLocationPropertyGenerator(item.Key,
                        propertyValueItem.GeoConstant != null ? propertyValueItem.GeoConstant : default(GeoLocationValue), variables);
                }

                if (propertyValueItem.GeoEnum != null)
                {
                    var lat = new EnumValues<decimal?>(propertyValueItem.GeoEnum.Select(e => (decimal?)e.Lat).ToArray());
                    var lon = new EnumValues<decimal?>(propertyValueItem.GeoEnum.Select(e => (decimal?)e.Lon).ToArray());
                    var alt = new EnumValues<decimal?>(propertyValueItem.GeoEnum.Select(e => (decimal?)e.Alt).ToArray());

                    var twoWay = propertyValueItem.IsTwoWay.HasValue ? propertyValueItem.IsTwoWay.Value : false;

                    propertyGenerator = new GeoLocationPropertyGenerator(item.Key, lat, lon, alt, variables, twoWay);
                }

                if (propertyValueItem.Random != null)
                {
                    var latRandom = propertyValueItem.LatRandomSequence != null ? new RandomRange(propertyValueItem.LatRandomSequence.IsDouble ?? true)
                    {
                        From = Convert.ToDecimal(propertyValueItem.LatRandomSequence.From, CultureInfo.InvariantCulture),
                        To = Convert.ToDecimal(propertyValueItem.LatRandomSequence.To, CultureInfo.InvariantCulture)
                    } : null;

                    var lonRandom = propertyValueItem.LonRandomSequence != null ? new RandomRange(propertyValueItem.LonRandomSequence.IsDouble ?? true)
                    {
                        From = Convert.ToDecimal(propertyValueItem.LonRandomSequence.From, CultureInfo.InvariantCulture),
                        To = Convert.ToDecimal(propertyValueItem.LonRandomSequence.To, CultureInfo.InvariantCulture)
                    } : null;

                    var altRandom = propertyValueItem.AltRandomSequence != null ? new RandomRange(propertyValueItem.AltRandomSequence.IsDouble ?? true)
                    {
                        From = Convert.ToDecimal(propertyValueItem.AltRandomSequence.From, CultureInfo.InvariantCulture),
                        To = Convert.ToDecimal(propertyValueItem.AltRandomSequence.To, CultureInfo.InvariantCulture)
                    } : null;

                    propertyGenerator = new GeoLocationPropertyGenerator(item.Key, GeoLocationType.Random, latRandom, lonRandom,
                        altRandom, variables, propertyValueItem.RoundDecimals);
                }
            }

            if (propertyValueItem.Type == TelemetryPropertyType.RadiusGeoLocation)
            {
                var points = propertyValueItem.Points != null ? propertyValueItem.Points : 36;
                var center = propertyValueItem.GeoConstant != null ? propertyValueItem.GeoConstant : default(GeoLocationValue);
                var radius = propertyValueItem.Radius != null ? propertyValueItem.Radius : 500;

                propertyGenerator = new RadiusGeoLocationGenerator(item.Key, points.Value, radius.Value, center, variables);
            }

            if (propertyGenerator != null)
            {
                propertyGenerator.Format = propertyValueItem?.Format;
            }

            propertyGenerator.PassTicks = propertyValueItem?.Pass.HasValue == true ? propertyValueItem.Pass.Value : 1;


            return propertyGenerator;
        }

        private IPropertyGenerator BuildDateTimeGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem,
            Dictionary<string, object> variables)
        {
            if (propertyValueItem.Constant != null)
            {
                return new DateTimeGeneratorBuilder()
                    .WithName(item.Key)
                    .WithConstant(propertyValueItem.Constant != null ?
                        DateTimeOffset.Parse(propertyValueItem.Constant.ToString()) : default(DateTimeOffset?))
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random != true)
            {
                return new DateTimeGeneratorBuilder()
                    .WithName(item.Key)
                    .WithSequence(new DateTimeOffsetSequence()
                    {
                        From = propertyValueItem.Sequence.From != null ?
                                DateTimeOffset.Parse(propertyValueItem.Sequence.From.ToString()) : DateTimeOffset.Now,
                        To = propertyValueItem.Sequence.To != null ?
                                DateTimeOffset.Parse(propertyValueItem.Sequence.To.ToString()) : DateTimeOffset.Now + TimeSpan.FromDays(10),
                        Step = propertyValueItem.Sequence.Step != null ?
                                TimeSpan.Parse(propertyValueItem.Sequence.Step.ToString()) : TimeSpan.FromMinutes(1)
                    })
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Enum != null)
            {
                return new DateTimeGeneratorBuilder()
                    .WithName(item.Key)
                    .WithEnumValue(new EnumValues<DateTimeOffset>(propertyValueItem.Enum.Where(i => i != null).Select(
                            f => string.IsNullOrEmpty(f) ? DateTimeOffset.Now : DateTimeOffset.Parse(f)
                        ).ToArray()), propertyValueItem.Random ?? true)
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random == true)
            {
                return new DateTimeGeneratorBuilder()
                    .WithName(item.Key)
                    .WithRandomRange(new DateTimeOffsetRandomSequence()
                    {
                        From = propertyValueItem.Sequence.From != null ?
                            DateTimeOffset.Parse(propertyValueItem.Sequence.From.ToString()) : DateTimeOffset.Now,
                        To = propertyValueItem.Sequence.To != null ?
                            DateTimeOffset.Parse(propertyValueItem.Sequence.To.ToString()) : DateTimeOffset.Now + TimeSpan.FromDays(10)
                    })
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.DateType != null)
            {
                return new DateTimeGeneratorBuilder()
                    .WithName(item.Key)
                    .WithType(propertyValueItem.DateType == "now" ? DateType.Now : DateType.UtcNow)
                    .WithVariables(variables)
                    .Build();
            }
            else
            {
                return new DateTimeGeneratorBuilder()
                    .WithName(item.Key)
                    .WithType(DateType.Now)
                    .WithVariables(variables)
                    .Build();
            }
        }

        private IPropertyGenerator BuildStringGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem,
            Dictionary<string, object> variables)
        {
            if (propertyValueItem.Constant != null)
            {
                return new StringGeneratorBuilder()
                    .WithName(item.Key)
                    .WithConstant(propertyValueItem.Constant.ToString())
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Enum != null)
            {
                return new StringGeneratorBuilder()
                    .WithName(item.Key)
                    .WithEnumValue(new EnumValues<string>(propertyValueItem.Enum.ToArray()), propertyValueItem.Random ?? true)
                    .WithNull(propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Guid != null)
            {
                return new StringGeneratorBuilder()
                    .WithName(item.Key)
                    .WithType(StringType.Guid)
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Placeholder != null)
            {
                return new StringGeneratorBuilder()
                    .WithName(item.Key)
                    .WithPlaceholder(propertyValueItem.Placeholder)
                    .WithVariables(variables)
                    .Build();
            }

            return new StringGeneratorBuilder()
                .WithName(item.Key)
                .WithConstant("string")
                .WithVariables(variables)
                .Build();
        }

        private IPropertyGenerator BuildTimestampGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem
            , Dictionary<string, object> variables)
        {
            if (propertyValueItem.Constant != null)
            {
                return new TimestampGeneratorBuilder()
                    .WithName(item.Key)
                    .WithConstant(Convert.ToInt64(propertyValueItem.Constant, CultureInfo.InvariantCulture))
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random != true)
            {
                return new TimestampGeneratorBuilder()
                    .WithName(item.Key)
                    .WithSequence(new NumberSequence()
                    {
                        From = Convert.ToDecimal(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                        To = Convert.ToDecimal(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture),
                        Step = propertyValueItem.Sequence.Step != null ? Convert.ToDecimal(propertyValueItem.Sequence.Step, CultureInfo.InvariantCulture) : 1
                    })
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.Enum != null)
            {
                return new TimestampGeneratorBuilder()
                    .WithName(item.Key)
                    .WithEnumValue(new EnumValues<long>(propertyValueItem.Enum.Select(
                        f => string.IsNullOrEmpty(f) ? 0 : Convert.ToInt64(f, CultureInfo.InvariantCulture)
                    ).ToArray()), propertyValueItem.Random ?? false)
                    .WithTwoWay(propertyValueItem.IsTwoWay ?? false)
                    .WithVariables(variables)
                    .Build();

            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random == true)
            {
                return new TimestampGeneratorBuilder()
                    .WithName(item.Key)
                    .WithRandomRange(new RandomRange()
                    {
                        From = Convert.ToInt64(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                        To = Convert.ToInt64(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture)
                    })
                    .WithVariables(variables)
                    .Build();
            }

            if (propertyValueItem.DateType != null)
            {
                return new TimestampGenerator(item.Key, propertyValueItem.DateType == "now" ? DateType.Now : DateType.UtcNow, variables);
            }
            else
            {
                return new TimestampGeneratorBuilder()
                    .WithName(item.Key)
                    .WithConstant(DateTimeOffset.Now.ToUnixTimeMilliseconds())
                    .WithVariables(variables)
                    .Build();
            }
        }
    }
}