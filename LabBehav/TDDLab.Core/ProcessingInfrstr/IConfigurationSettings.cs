namespace TDDLab.Core.Infrastructure
{
    public interface IConfigurationSettings
    {
        string GetSettingsByKey(string key);
    }
}