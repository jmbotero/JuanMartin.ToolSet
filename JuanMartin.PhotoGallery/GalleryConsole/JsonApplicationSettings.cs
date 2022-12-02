using Microsoft.Extensions.Configuration;
using System.IO;

namespace JuanMartin.Sandbox
{
    public class JsonApplicationSettings
    {
        public JsonApplicationSettings(string appSettingsPath)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(appSettingsPath, "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var Configuration = configurationBuilder.Build();
            ConnectionString = Configuration.GetSection("ConnectionString").GetSection("DefaultConnection").Value;
            DataLoadConnectionString = Configuration.GetSection("ConnectionString").GetSection("DataLoadConnection").Value;
        }

        public string ConnectionString { get; set; }
        public string DataLoadConnectionString { get; set; }
        public IConfiguration Configuration { get; private set; }
    }
}
