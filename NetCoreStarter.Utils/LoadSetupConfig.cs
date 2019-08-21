using Newtonsoft.Json;
using System;
using System.IO;

namespace NetCoreStarter.Utils.Classes
{
    public static class LoadSetupConfig
    {
        public static void LoadSettings()
        {
            AppConfig.Setting = JsonConvert.DeserializeObject<Setting>
                (File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppConfig.json")));
        }
    }
}
