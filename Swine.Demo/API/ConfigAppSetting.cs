using System.Configuration;

namespace Swine.Demo.API
{
    public class ConfigAppSetting
    {
        public static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Lưu thông tin cấu hình
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
