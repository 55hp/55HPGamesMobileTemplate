using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using hp55games.Mobile.Core.Architecture;
using UnityEngine;

namespace hp55games.Mobile.Core.Localization
{
    public sealed class LocalizationService : ILocalizationService
    {
        public event Action LanguageChanged;

        public string CurrentLanguage { get; private set; } = "en";

        // lang -> (key -> value)
        private readonly Dictionary<string, Dictionary<string, string>> _tables =
            new Dictionary<string, Dictionary<string, string>>();

        private readonly string _defaultLanguage = "en";

        public LocalizationService()
        {
            LoadTablesFromResources("Localization/localization_master");
            Debug.Log("[LocalizationService] Initialized from TSV.");
        }

        private void LoadTablesFromResources(string resourcePath)
        {
            var asset = Resources.Load<TextAsset>(resourcePath);
            if (asset == null)
            {
                Debug.LogError($"[LocalizationService] TextAsset not found at Resources/{resourcePath}. Using empty tables.");
                return;
            }

            using var reader = new StringReader(asset.text);

            // 1) Header
            var headerLine = reader.ReadLine();
            if (string.IsNullOrEmpty(headerLine))
            {
                Debug.LogError("[LocalizationService] TSV header line is empty.");
                return;
            }

            var headerCols = headerLine.Split('\t');
            if (headerCols.Length < 2)
            {
                Debug.LogError("[LocalizationService] TSV header must have at least 'key' and one language.");
                return;
            }

            // Colonna 0 = "key", le altre sono lingue
            var languages = new List<string>();
            for (int i = 1; i < headerCols.Length; i++)
            {
                var lang = headerCols[i].Trim();
                if (string.IsNullOrEmpty(lang)) continue;
                languages.Add(lang);
                if (!_tables.ContainsKey(lang))
                    _tables[lang] = new Dictionary<string, string>();
            }

            // 2) Righe
            string line;
            int lineNumber = 1;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var cols = line.Split('\t');

                if (cols.Length == 0) continue;

                var key = cols[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;

                for (int i = 1; i < cols.Length && i <= languages.Count; i++)
                {
                    var lang = languages[i - 1];
                    var value = cols[i];

                    if (!_tables.TryGetValue(lang, out var table))
                    {
                        table = new Dictionary<string, string>();
                        _tables[lang] = table;
                    }

                    if (table.ContainsKey(key))
                    {
                        Debug.LogWarning($"[LocalizationService] Duplicate key '{key}' for lang '{lang}' at line {lineNumber}. Overwriting.");
                    }

                    table[key] = value;
                }
            }

            Debug.Log($"[LocalizationService] Loaded localization for languages: {string.Join(", ", languages)}");
        }

        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                languageCode = _defaultLanguage;

            if (!_tables.ContainsKey(languageCode))
            {
                Debug.LogWarning($"[LocalizationService] Language '{languageCode}' not found. Falling back to '{_defaultLanguage}'.");
                languageCode = _defaultLanguage;
            }

            if (CurrentLanguage == languageCode)
                return;

            CurrentLanguage = languageCode;
            Debug.Log($"[LocalizationService] Language set to '{CurrentLanguage}'.");

            LanguageChanged?.Invoke();
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (_tables.TryGetValue(CurrentLanguage, out var table) &&
                table.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fallback: prova default language
            if (CurrentLanguage != _defaultLanguage &&
                _tables.TryGetValue(_defaultLanguage, out var defTable) &&
                defTable.TryGetValue(key, out var defValue))
            {
                Debug.LogWarning($"[LocalizationService] Missing key '{key}' for lang '{CurrentLanguage}', using default '{_defaultLanguage}'.");
                return defValue;
            }

            Debug.LogWarning($"[LocalizationService] Missing key '{key}' for lang '{CurrentLanguage}'.");
            return key;
        }

        public string Get(string key, params object[] args)
        {
            var format = Get(key);
            try
            {
                return string.Format(CultureInfo.InvariantCulture, format, args);
            }
            catch
            {
                Debug.LogWarning($"[LocalizationService] Format error for key '{key}' with value '{format}'.");
                return format;
            }
        }
    }
}
