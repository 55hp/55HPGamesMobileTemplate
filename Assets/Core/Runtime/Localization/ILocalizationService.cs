using System;
using System.Globalization;

namespace hp55games.Mobile.Core.Localization
{
    public interface ILocalizationService
    {
        string CurrentLanguage { get; }

        void SetLanguage(string languageCode);

        string Get(string key);
        string Get(string key, params object[] args);

        event Action LanguageChanged;
    }
}