using EntityFX.IotSimulator.Engine.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityFX.IotSimulator.Engine.Builder
{
    public static class BuilderHelper
    {
        public static TBuilder GetBuilder<TBuilder, TType, TStubBuilder>(this IBuilder<TType> builder,
            string assemblyName, string senderTypeName, ILogger logger, IConfiguration configuration, Dictionary<string, object> settings)
            where TStubBuilder : IBuilder<TType>
            where TBuilder : IBuilder<TType>
        {
            return (TBuilder)builder.GetBuilder<TType, TStubBuilder>(assemblyName, senderTypeName, logger, configuration, settings);
        }


        public static IBuilder<TType> GetBuilder<TType, TStubBuilder>(this IBuilder<TType> builder,
            string assemblyName, string senderTypeName, ILogger logger, IConfiguration configuration, Dictionary<string, object> settings)
            where TStubBuilder : IBuilder<TType>
        {

            if (assemblyName != null)
            {
                var customAssembly = GetOrLoadAssembly(assemblyName);
                builder = BuildType<TType>(customAssembly, senderTypeName, logger, configuration, settings);

                return builder;
            }

            builder = BuildType<TType>(typeof(TType).Assembly, senderTypeName, logger, configuration, settings);


            if (builder != null)
            {
                return builder;
            }

            builder = BuildType<TType>(Assembly.GetEntryAssembly(), senderTypeName, logger, configuration, settings);

            if (builder != null)
            {
                return builder;
            }
            return (TStubBuilder)Activator.CreateInstance(typeof(TStubBuilder), logger, configuration);
        }

        public static TType Build<TType, TStubBuilder>(this IBuilder<TType> builder,
            string assemblyName, string senderTypeName, ILogger logger, IConfiguration configuration, Dictionary<string, object> settings)
            where TStubBuilder : IBuilder<TType>
        {
            return builder.GetBuilder<TType, TStubBuilder>(assemblyName, senderTypeName, logger, configuration, settings).Build();
        }

        public static (string AssemblyName, string TypeName) GetDefaultAssemblyAndTypeName<TDefaultType>(PluginSettings pluginSettings)
        {
            var typeName = pluginSettings?.Type ?? typeof(TDefaultType).Name;
            var assemblyFile = pluginSettings?.Assembly ?? Assembly.GetAssembly(typeof(TDefaultType)).GetName().Name;

            return (assemblyFile, typeName);
        }

        private static Assembly GetOrLoadAssembly(string assemblyName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var asm = assemblies.FirstOrDefault(ass => ass.GetName().Name == assemblyName || ass.GetName().FullName == assemblyName);
            if (asm != null)
            {
                return asm;
            }
            return Assembly.LoadFrom(assemblyName);
        }

        private static IBuilder<TType> BuildType<TType>(Assembly assembly,
            string senderTypeName, ILogger logger, IConfiguration configuration, Dictionary<string, object> settings)
        {
            var loadedTypes = assembly.GetTypes();

            var senderType = loadedTypes.FirstOrDefault(t => t.FullName == senderTypeName || t.Name == senderTypeName);

            if (senderType != null)
            {
                return Activator.CreateInstance(senderType,
                    new object[] { logger, configuration, settings }) as IBuilder<TType>;
            }

            return null;
        }
    }
}
