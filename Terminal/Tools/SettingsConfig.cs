using Microsoft.Extensions.Configuration;
using Search.Models;

namespace AutoSearch.Tools
{
    public static class SettingsConfig
    {
        
        public static MousePosition mousePosition;
        public static ThreadSleep threadSleep;

        public static void LoadConfig()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var Configuration = builder.Build();
        }


        public static MousePosition GetSetting(string key)
        {

            //return Configuration[key];


            //var setting = Configuration.GetSection("MousePosition").Get<MousePosition>();
            LoadConfig();
            return null;
        }
    }
}
