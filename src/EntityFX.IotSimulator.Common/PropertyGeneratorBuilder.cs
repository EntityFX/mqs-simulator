using EntityFX.IotSimulator.Engine;
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

        private IPropertyGenerator BuildBoolGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem)
        {
            if (propertyValueItem.Constant != null)
            {
                return new BoolGenerator(item.Key, Convert.ToBoolean(propertyValueItem.Constant, CultureInfo.InvariantCulture));
            }

            if (propertyValueItem.Random != null)
            {
                return new BoolGenerator(item.Key, BoolType.Random, propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false);
            }

            return new BoolGenerator(item.Key, BoolType.Sequece, propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false);
        }

        private IPropertyGenerator BuildNumberGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem)
        {
            if (propertyValueItem.Constant != null)
            {
                return new NumberGenerator(item.Key, Convert.ToDecimal(propertyValueItem.Constant, CultureInfo.InvariantCulture))
                {
                    RoundDecimals = propertyValueItem.RoundDecimals
                };
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random != true)
            {
                return new NumberGenerator(item.Key, new NumberSequence()
                {
                    From = Convert.ToDecimal(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                    To = Convert.ToDecimal(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture),
                    Step = propertyValueItem.Sequence.Step != null ? Convert.ToDecimal(propertyValueItem.Sequence.Step, CultureInfo.InvariantCulture) : 1,
                    IsTwoWay = propertyValueItem.IsTwoWay.HasValue ? propertyValueItem.IsTwoWay.Value : false
                }, propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                {
                    RoundDecimals = propertyValueItem.RoundDecimals
                };
            }

            if (propertyValueItem.Enum != null)
            {
                return new NumberGenerator(item.Key,
                    new EnumValues<decimal?>(propertyValueItem.Enum.Select(
                        f => string.IsNullOrEmpty(f) ? null : (decimal?)Convert.ToDecimal(f, CultureInfo.InvariantCulture)
                    ).ToArray()), propertyValueItem.Random ?? false,
                    propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false,
                    propertyValueItem.IsTwoWay ?? false
                )
                {
                    RoundDecimals = propertyValueItem.RoundDecimals
                };
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random == true)
            {
                if (propertyValueItem.Sequence?.IsDouble == true)
                {
                    return new NumberGenerator(item.Key, new RandomRange()
                    {
                        From = Convert.ToDecimal(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                        To = Convert.ToDecimal(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture),
                        IsDouble = true
                    }, propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                    {
                        RoundDecimals = propertyValueItem.RoundDecimals
                    };
                }
                return new NumberGenerator(item.Key, new RandomRange()
                {
                    From = Convert.ToInt64(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                    To = Convert.ToInt64(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture),
                    IsDouble = propertyValueItem.Sequence?.IsDouble ?? false
                }, propertyValueItem.UseNull != null ? propertyValueItem.UseNull.Value : false)
                {
                    RoundDecimals = propertyValueItem.RoundDecimals
                };
            }
            else
            {
                return new NumberGenerator(item.Key, 1)
                {
                    RoundDecimals = propertyValueItem.RoundDecimals
                };
            }
        }

        public IPropertyGenerator BuildPropertyGenerator(KeyValuePair<string, TelemetryPropertySetting> item)
        {
            IPropertyGenerator propertyGenerator = null;
            var propertyValueItem = item.Value;

            if (propertyValueItem.Type == TelemetryPropertyType.Number)
            {
                propertyGenerator = BuildNumberGenerator(item, propertyValueItem);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.String)
            {
                propertyGenerator = BuildStringGenerator(item, propertyValueItem);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.Bool)
            {
                propertyGenerator = BuildBoolGenerator(item, propertyValueItem);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.Timestamp)
            {
                propertyGenerator = BuildTimestampGenerator(item, propertyValueItem);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.DateTime)
            {
                propertyGenerator = BuildDateTimeGenerator(item, propertyValueItem);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.Complex)
            {
                propertyGenerator = new ComplexPropertyGenerator(item.Key, propertyValueItem.Properties.Select(p => BuildPropertyGenerator(p)).ToArray());
            }

            if (propertyValueItem.Type == TelemetryPropertyType.Placeholder)
            {
                string template = string.Empty;
                if (File.Exists(propertyValueItem.TemplatePath))
                {
                    template = File.ReadAllText(propertyValueItem.TemplatePath);
                }
                propertyGenerator = new PlaceholderPropertyGenerator(item.Key, propertyValueItem.Properties.Select(p => BuildPropertyGenerator(p)).ToArray(), template);
            }

            if (propertyValueItem.Type == TelemetryPropertyType.GeoLocation)
            {
                if (propertyValueItem.GeoConstant != null)
                {
                    propertyGenerator = new GeoLocationPropertyGenerator(item.Key, propertyValueItem.GeoConstant != null ? propertyValueItem.GeoConstant : default(GeoLocationValue));
                }

                if (propertyValueItem.GeoEnum != null)
                {
                    var lat = new EnumValues<decimal?>(propertyValueItem.GeoEnum.Select(e => (decimal?)e.Lat).ToArray());
                    var lon = new EnumValues<decimal?>(propertyValueItem.GeoEnum.Select(e => (decimal?)e.Lon).ToArray());
                    var alt = new EnumValues<decimal?>(propertyValueItem.GeoEnum.Select(e => (decimal?)e.Alt).ToArray());

                    var twoWay = propertyValueItem.IsTwoWay.HasValue ? propertyValueItem.IsTwoWay.Value : false;

                    propertyGenerator = new GeoLocationPropertyGenerator(item.Key, lat, lon, alt, twoWay);
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

                    propertyGenerator = new GeoLocationPropertyGenerator(item.Key, GeoLocationType.Random, latRandom, lonRandom, altRandom, propertyValueItem.RoundDecimals);
                }
            }

            if (propertyValueItem.Type == TelemetryPropertyType.RadiusGeoLocation)
            {
                var points = propertyValueItem.Points != null ? propertyValueItem.Points : 36;
                var center = propertyValueItem.GeoConstant != null ? propertyValueItem.GeoConstant : default(GeoLocationValue);
                var radius = propertyValueItem.Radius != null ? propertyValueItem.Radius : 500;

                propertyGenerator = new RadiusGeoLocationGenerator(item.Key, points.Value, radius.Value, center );
            }

            if (propertyGenerator != null)
            {
                propertyGenerator.Format = propertyValueItem?.Format;
            }

            propertyGenerator.PassTicks = propertyValueItem?.Pass.HasValue == true ? propertyValueItem.Pass.Value : 1;


            return propertyGenerator;
        }

        private IPropertyGenerator BuildDateTimeGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem)
        {
            if (propertyValueItem.Constant != null)
            {
                return new DateTimeGenerator(item.Key, propertyValueItem.Constant != null ? DateTimeOffset.Parse(propertyValueItem.Constant.ToString()) : default(DateTimeOffset?));
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random != true)
            {
                return new DateTimeGenerator(item.Key, new DateTimeOffsetSequence()
                {
                    From = propertyValueItem.Sequence.From != null ? DateTimeOffset.Parse(propertyValueItem.Sequence.From.ToString()) : DateTimeOffset.Now,
                    To = propertyValueItem.Sequence.To != null ? DateTimeOffset.Parse(propertyValueItem.Sequence.To.ToString()) : DateTimeOffset.Now + TimeSpan.FromDays(10),
                    Step = propertyValueItem.Sequence.Step != null ? TimeSpan.Parse(propertyValueItem.Sequence.Step.ToString()) : TimeSpan.FromMinutes(1)
                });
            }

            if (propertyValueItem.Enum != null)
            {
                return new DateTimeGenerator(item.Key,
                    new EnumValues<DateTimeOffset>(propertyValueItem.Enum.Where(i => i != null).Select(
                        f => string.IsNullOrEmpty(f) ? DateTimeOffset.Now : DateTimeOffset.Parse(f)
                    ).ToArray()), propertyValueItem.Random ?? true
                );
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random == true)
            {
                return new DateTimeGenerator(item.Key, new DateTimeOffsetRandomSequence()
                {
                    From = propertyValueItem.Sequence.From != null ? DateTimeOffset.Parse(propertyValueItem.Sequence.From.ToString()) : DateTimeOffset.Now,
                    To = propertyValueItem.Sequence.To != null ? DateTimeOffset.Parse(propertyValueItem.Sequence.To.ToString()) : DateTimeOffset.Now + TimeSpan.FromDays(10)
                });
            }

            if (propertyValueItem.DateType != null)
            {
                return new DateTimeGenerator(item.Key, propertyValueItem.DateType == "now" ? DateType.Now : DateType.UtcNow);
            }
            else
            {
                return new DateTimeGenerator(item.Key, DateTimeOffset.Now);
            }
        }

        private IPropertyGenerator BuildStringGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem)
        {
            if (propertyValueItem.Constant != null)
            {
                return new StringGenerator(item.Key, propertyValueItem.Constant.ToString());
            }

            if (propertyValueItem.Enum != null)
            {
                return new StringGenerator(item.Key,
                    new EnumValues<string>(propertyValueItem.Enum.ToArray()
                    ), propertyValueItem.Random ?? true
                );
            }

            if (propertyValueItem.Guid != null)
            {
                return new StringGenerator(item.Key, StringType.Guid);
            }

            return new StringGenerator(item.Key, "string");
        }

        private IPropertyGenerator BuildTimestampGenerator(KeyValuePair<string, TelemetryPropertySetting> item, TelemetryPropertySetting propertyValueItem)
        {
            if (propertyValueItem.Constant != null)
            {
                return new NumberGenerator(item.Key, Convert.ToDecimal(propertyValueItem.Constant, CultureInfo.InvariantCulture));
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random != true)
            {
                return new NumberGenerator(item.Key, new NumberSequence()
                {
                    From = Convert.ToDecimal(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                    To = Convert.ToDecimal(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture),
                    Step = propertyValueItem.Sequence.Step != null ? Convert.ToDecimal(propertyValueItem.Sequence.Step, CultureInfo.InvariantCulture) : 1
                });
            }

            if (propertyValueItem.Enum != null)
            {
                return new NumberGenerator(item.Key,
                    new EnumValues<decimal?>(propertyValueItem.Enum.Select(
                        f => string.IsNullOrEmpty(f) ? null : (decimal?)Convert.ToDecimal(f, CultureInfo.InvariantCulture)
                    ).ToArray()), propertyValueItem.Random ?? true, 
                    propertyValueItem.IsTwoWay ?? false
                );
            }

            if (propertyValueItem.Sequence != null && propertyValueItem.Random == true)
            {
                return new NumberGenerator(item.Key, new RandomRange()
                {
                    From = Convert.ToInt64(propertyValueItem.Sequence.From, CultureInfo.InvariantCulture),
                    To = Convert.ToInt64(propertyValueItem.Sequence.To, CultureInfo.InvariantCulture)
                });
            }

            if (propertyValueItem.DateType != null)
            {
                return new TimestampGenerator(item.Key, propertyValueItem.DateType == "now" ? DateType.Now : DateType.UtcNow);
            }
            else
            {
                return new TimestampGenerator(item.Key, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            }
        }
    }
}