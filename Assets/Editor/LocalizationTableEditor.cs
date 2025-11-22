// Assets/Core/Editor/Localization/LocalizationTableEditor.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace hp55games.Mobile.Core.Editor.Tools
{
    public class LocalizationTableEditor : EditorWindow
    {
        // Percorso del TSV nel progetto
        private const string TsvAssetPath = "Assets/Resources/Localization/localization_master.txt";

        private class Row
        {
            public string Key;
            public string[] Values; // una per lingua
        }

        private string[] _headers;    // es: key, en, it, ...
        private List<Row> _rows = new();
        private Vector2 _scroll;
        private string _filterPrefix = string.Empty;
        private bool _dirty;

        [MenuItem("hp55games Tools/Localization Table Editor")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<LocalizationTableEditor>("Localization Table");
            wnd.minSize = new Vector2(800, 400);
            wnd.LoadTsv();
        }

        private void OnGUI()
        {
            if (_headers == null || _headers.Length == 0)
            {
                if (GUILayout.Button("Load TSV"))
                    LoadTsv();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reload", GUILayout.Width(80)))
                LoadTsv();

            if (GUILayout.Button("Save", GUILayout.Width(80)))
                SaveTsv();

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Filter prefix (e.g. 'ui.', 'game.')", GUILayout.Width(200));
            _filterPrefix = EditorGUILayout.TextField(_filterPrefix);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);

            // Header row
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            for (int i = 0; i < _headers.Length; i++)
            {
                EditorGUILayout.LabelField(_headers[i], EditorStyles.boldLabel);
            }
            EditorGUILayout.EndHorizontal();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var row in GetFilteredRows())
            {
                EditorGUILayout.BeginHorizontal();
                // key non la facciamo cambiare per evitare casini coi prefab
                EditorGUILayout.LabelField(row.Key, GUILayout.Width(220));

                for (int i = 1; i < _headers.Length; i++)
                {
                    string old = row.Values[i - 1] ?? string.Empty;
                    string updated = EditorGUILayout.TextField(old);
                    if (!ReferenceEquals(old, updated) && old != updated)
                    {
                        row.Values[i - 1] = updated;
                        _dirty = true;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (_dirty)
            {
                EditorGUILayout.HelpBox("You have unsaved changes.", MessageType.Info);
            }
        }

        private IEnumerable<Row> GetFilteredRows()
        {
            if (string.IsNullOrWhiteSpace(_filterPrefix))
                return _rows;

            return _rows.Where(r => r.Key.StartsWith(_filterPrefix, StringComparison.OrdinalIgnoreCase));
        }

        private void LoadTsv()
        {
            _rows.Clear();
            _headers = Array.Empty<string>();
            _dirty = false;

            if (!File.Exists(TsvAssetPath))
            {
                Debug.LogError("[LocalizationTableEditor] TSV not found at: " + TsvAssetPath);
                return;
            }

            var allLines = File.ReadAllLines(TsvAssetPath);
            if (allLines.Length == 0)
            {
                Debug.LogError("[LocalizationTableEditor] TSV is empty.");
                return;
            }

            // Prima linea: header
            _headers = allLines[0].Split('\t');

            for (int i = 1; i < allLines.Length; i++)
            {
                var line = allLines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("#")) continue; // salta commenti

                var parts = line.Split('\t');
                if (parts.Length == 0) continue;

                var key = parts[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;

                // il resto sono i valori delle lingue
                var values = new string[_headers.Length - 1];
                for (int c = 1; c < _headers.Length; c++)
                {
                    values[c - 1] = c < parts.Length ? parts[c] : string.Empty;
                }

                _rows.Add(new Row
                {
                    Key = key,
                    Values = values
                });
            }

            Debug.Log($"[LocalizationTableEditor] Loaded {_rows.Count} rows from TSV.");
        }

        private void SaveTsv()
        {
            if (_headers == null || _headers.Length == 0)
                return;

            var lines = new List<string>();

            // header
            lines.Add(string.Join("\t", _headers));

            // rows
            foreach (var row in _rows)
            {
                var cols = new List<string> { row.Key };
                cols.AddRange(row.Values.Select(v => v ?? string.Empty));
                lines.Add(string.Join("\t", cols));
            }

            File.WriteAllLines(TsvAssetPath, lines);
            AssetDatabase.Refresh();
            _dirty = false;

            Debug.Log($"[LocalizationTableEditor] Saved {_rows.Count} rows to TSV.");
        }
    }
}
