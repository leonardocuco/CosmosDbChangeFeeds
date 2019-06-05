using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using ChangeFeedCosmosDB;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using ChangeFeedCosmosDB.Drivers;

[assembly: FunctionsStartup(typeof(Startup))]
namespace ChangeFeedCosmosDB
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = GetConfigurationRoot();

            builder.Services.AddTransient<ApiDriver>();
            ConfigureSerializationSettings();

        }

        private static IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true)
                .Build();
        }

        private static void ConfigureSerializationSettings()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var jsonSettings = new JsonSerializerSettings();
                return jsonSettings;
            };
        }
    }
}
