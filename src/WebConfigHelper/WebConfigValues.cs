using System;
using System.Globalization;
using System.Linq;
using Validation;

[assembly: CLSCompliant(true)]
namespace WebConfigHelper
{
    public class WebConfigValues
    {
        private IWebConfigProvider _webConfigProvider;

        public WebConfigValues()
        {
            _webConfigProvider = new WebConfigurationManagerProvider();
        }

        public WebConfigValues(IWebConfigProvider webConfigProvider)
        {
            _webConfigProvider = webConfigProvider;
        }

        public T[] GetAppSettingArray<T>(string key, T[] defaultValue)
        {
            Requires.NotNullOrWhiteSpace(key, nameof(key));
            Requires.NotNull(defaultValue, nameof(defaultValue));

            return GetAppSettingArray<T>(key);
        }

        public T[] GetAppSettingArray<T>(string key)
        {
            Requires.NotNullOrWhiteSpace(key, nameof(key));

            var settingValue = GetAppSetting<string>(key);

            var values = settingValue
                .Split(',')
                .Select(x => ChangeType<T>(x))
                .ToArray();

            return values;
        }

        public T GetAppSetting<T>(string key, T defaultValue)
        {
            Requires.NotNullOrWhiteSpace(key, nameof(key));

            T settingValue;

            try
            {
                settingValue = GetAppSetting<T>(key);
            }
            catch (FormatException)
            {
                settingValue = defaultValue;
            }
            catch (OverflowException)
            {
                settingValue = defaultValue;
            }
            catch (ArgumentNullException)
            {
                settingValue = defaultValue;
            }
            catch
            {
                throw;
            }

            return settingValue == null ? defaultValue : settingValue;
        }

        public T GetAppSetting<T>(string key)
        {
            Requires.NotNullOrWhiteSpace(key, nameof(key));

            var setting = _webConfigProvider.GetAppSetting(key);
            T typedValue = default(T);

            try
            {
                typedValue = ChangeType<T>(setting);
            }
            catch (FormatException exception)
            {
                throw new FormatException(StringFormat.ToInvariant($"Setting '{key}' was not in the correct format. Expected type {typeof(T)}. Setting value = {setting}"), exception);
            }
            catch (OverflowException exception)
            {
                throw new OverflowException(StringFormat.ToInvariant($"Setting '{key}' caused an overflow. Expected type {typeof(T)}. Setting value = {setting}"), exception);
            }
            catch (InvalidCastException exception)
            {
                if (setting == null)
                {
                    throw new ArgumentNullException(StringFormat.ToInvariant($"Setting '{key}' returned null and type {typeof(T)} cannot have a null value"), exception);
                }
                else
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }

            return typedValue;
        }

        private static T ChangeType<T>(string obj)
        {
            if (obj == null && IsOfNullableType<T>())
            {
                return default(T);
            }
            else
            {
                return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.CurrentCulture);
            }
        }

        private static bool IsOfNullableType<T>()
        {
            return Nullable.GetUnderlyingType(typeof(T)) != null;
        }
    }
}