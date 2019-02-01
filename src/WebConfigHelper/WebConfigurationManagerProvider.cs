using System.Web.Configuration;

namespace WebConfigHelper
{
    public class WebConfigurationManagerProvider : IWebConfigProvider
    {
        public string GetAppSetting(string key)
        {
            return WebConfigurationManager.AppSettings[key];
        }
    }
}