using System;
using Windows.UI.Xaml.Data;
using AzureKeyVaultManager.Contracts;

namespace AzureKeyVaultManager.UWP.Converters
{
    public class ValidityBriefConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
                var secret = value as IKeyVaultSecret;

            if (secret == null) return "";

            if (secret.Expires != null && secret.Expires <= DateTime.Now)
                return $"Expired at {secret.Expires.Value.ToString("g")}";
            else if (secret.ValidAfter != null && secret.ValidAfter > DateTime.Now)
            {
                if (secret.Expires != null)
                    return $"Valid {secret.ValidAfter.Value.ToString("g")} to {secret.Expires.Value.ToString("g")}";
                else
                    return $"Valid starting {secret.ValidAfter.Value.ToString("g")}";
            }
            else if (secret.Expires != null && secret.Expires > DateTime.Now)
                return $"Valid until {secret.Expires.Value.ToString("g")}";

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
