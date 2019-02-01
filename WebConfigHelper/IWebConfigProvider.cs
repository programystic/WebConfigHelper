namespace WebConfigHelper
{
    public interface IWebConfigProvider
    {
        string GetAppSetting(string key);
    }
}
