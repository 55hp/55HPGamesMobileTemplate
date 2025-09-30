using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace hp55games.Editor
{
    public static class TemplateAudit
    {
        [MenuItem("Tools/hp55games/Export Template Audit")]
        public static void Export()
        {
            var outPath = Path.Combine(Application.dataPath, "../TemplateAudit.md");
            using (var w = new StreamWriter(outPath, false))
            {
                w.WriteLine("# hp55games – Template Audit");
                w.WriteLine($"Generated: {System.DateTime.Now}");
                w.WriteLine();

                // Unity version
                w.WriteLine("## Unity/Project");
                w.WriteLine($"- Application.unityVersion: `{Application.unityVersion}`");
                w.WriteLine();

                // ProjectSettings quick checks
                w.WriteLine("## ProjectSettings checks");
                var rootNs = UnityEditor.EditorSettings.projectGenerationRootNamespace;
                w.WriteLine($"- Root namespace: `{rootNs}` (expected: `hp55games`)");
                w.WriteLine($"- Active build target: {EditorUserBuildSettings.activeBuildTarget}");
                w.WriteLine();

                // Packages manifest
                w.WriteLine("## Packages/manifest.json (top-level)");
                var manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
                if (File.Exists(manifestPath))
                {
                    var lines = File.ReadAllLines(manifestPath)
                        .Where(l => l.Contains("\"com.") || l.Contains("\"org.") || l.Contains("\"com.unity."));
                    foreach (var l in lines) w.WriteLine("- " + l.Trim());
                }
                else w.WriteLine("- manifest.json not found");
                w.WriteLine();

                // .asmdef
                w.WriteLine("## Assembly Definitions (*.asmdef)");
                var asmdefs = AssetDatabase.FindAssets("t:asmdef")
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .OrderBy(p => p);
                foreach (var a in asmdefs) w.WriteLine("- " + a);
                w.WriteLine();

                // Scenes
                w.WriteLine("## Scenes in Assets/Scenes");
                var scenes = AssetDatabase.FindAssets("t:Scene", new[] {"Assets/Scenes"})
                    .Select(AssetDatabase.GUIDToAssetPath).OrderBy(p => p);
                foreach (var s in scenes) w.WriteLine("- " + s);
                w.WriteLine();

                // UI prefabs quick list
                w.WriteLine("## UI Prefabs (Assets/UI/Prefabs)");
                var uiPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] {"Assets/UI/Prefabs"})
                    .Select(AssetDatabase.GUIDToAssetPath).OrderBy(p => p);
                foreach (var p in uiPrefabs) w.WriteLine("- " + p);
                if (!uiPrefabs.Any()) w.WriteLine("- (none)");
                w.WriteLine();

                // Presence checks (class names you abbiamo definito)
                w.WriteLine("## Presence checks (core roles with aliases)");
                var roleMap = new Dictionary<string, string[]>
                {
                    // Role → candidati (metti prima i tuoi)
                    ["UiManager"] = new[] {
                        "hp55games.Ui.UiManager",                      // standard audit
                        "hp55games.Mobile.Core.Architecture.UIManager"// se in futuro lo chiami così
                    },
                    ["ConfigService"] = new[] {
                        "hp55games.Mobile.Core.Architecture.ConfigService",
                        "hp55games.Config.ConfigService"
                    },
                    ["EventBus"] = new[] {
                        "hp55games.Mobile.Core.Architecture.EventBus",
                        "hp55games.Core.EventBus"
                    },
                    ["SaveService"] = new[] {
                        "hp55games.Mobile.Core.Architecture.SaveService",
                        "hp55games.Services.SaveService"
                    },
                    ["Logger"] = new[] {
                        "hp55games.Mobile.Core.Architecture.UnityLog",
                        "hp55games.Core.Logger"
                    },
                };

                foreach (var kv in roleMap)
                {
                    var role = kv.Key;
                    var candidates = kv.Value;
                    var t = TypeFinder.FindFirst(candidates);
                    if (t != null)
                        w.WriteLine($"- {role}: FOUND → `{t.FullName}`");
                    else
                        w.WriteLine($"- {role}: MISSING (tried: {string.Join(", ", candidates)})");
                }
                w.WriteLine();


                // Tests
                w.WriteLine("## Tests folders");
                var testFolders = new[] {"Assets/Tests/EditMode", "Assets/Tests/PlayMode"};
                foreach (var folder in testFolders)
                {
                    var exists = AssetDatabase.IsValidFolder(folder);
                    w.WriteLine($"- {folder}: {(exists ? "present" : "missing")}");
                }

                w.Flush();
            }

            EditorUtility.RevealInFinder(Path.GetFullPath(outPath));
            Debug.Log($"[hp55games] Template audit exported to: {outPath}");
        }
    }
}


static class TypeFinder
{
    public static Type FindFirst(params string[] fullyQualifiedNames)
    {
        var asms = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var fqn in fullyQualifiedNames)
        {
            if (string.IsNullOrWhiteSpace(fqn)) continue;
            var t = asms.SelectMany(a => SafeGetTypes(a)).FirstOrDefault(x => x.FullName == fqn);
            if (t != null) return t;
        }
        return null;
    }

    static IEnumerable<Type> SafeGetTypes(Assembly a)
    {
        try { return a.GetTypes(); } catch { return Array.Empty<Type>(); }
    }
}
