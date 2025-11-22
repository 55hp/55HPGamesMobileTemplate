// Assets/Editor/HP55_StaticScan.cs
// Static scan di pattern rischiosi nei .cs (runtime). Genera StaticScan.md in root.
// Cerca: FindObjectOfType in Update, Resources.Load runtime, public fields esposti,
// async void, catch(Exception) vuoti, DestroyImmediate runtime, Singleton hard, PlayerPrefs raw,
// LoadScene senza Additive.
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class HP55_StaticScan
{
    [MenuItem("hp55games Tools/Static Scan/Run")]
    public static void Run()
    {
        var outPath = Path.Combine(Application.dataPath, "../StaticScan.md");
        var sb = new StringBuilder();
        sb.AppendLine("# hp55games – Static Scan");
        sb.AppendLine($"Generated: {DateTime.Now}");
        sb.AppendLine();

        var scripts = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Replace("\\","/").Contains("/Editor/")) // solo runtime
            .ToArray();

        var patterns = new (string key, Regex rx, string why)[]
        {
            ("FindObjectOfType_in_Update", new Regex(@"void\s+Update\s*\([\s\S]*?FindObjectOfType<", RegexOptions.Multiline), "FindObjectOfType in Update: costoso, fai caching o inject."),
            ("Resources_Load", new Regex(@"Resources\.Load\s*<", RegexOptions.Multiline), "Resources.Load a runtime: preferisci Addressables o pre-ref."),
            ("Public_Fields", new Regex(@"\bpublic\s+(?!class|struct|enum|interface|static)\s+[\w\?<>\[\]]+\s+\w+\s*;", RegexOptions.Multiline), "Campi public nei MonoBehaviour: usa [SerializeField] private."),
            ("Async_Void", new Regex(@"\basync\s+void\b"), "async void: non cattura eccezioni; usa async Task."),
            ("Empty_Catch", new Regex(@"catch\s*\(\s*Exception[^\)]*\)\s*\{\s*\}", RegexOptions.Multiline), "catch(Exception) vuoto: logga almeno l'errore."),
            ("DestroyImmediate_Runtime", new Regex(@"DestroyImmediate\s*\(", RegexOptions.Multiline), "DestroyImmediate in runtime: rischioso, usa Destroy."),
            ("Hard_Singleton", new Regex(@"static\s+[^;\n]*\bInstance\b|Instance\s*=\s*this|DontDestroyOnLoad\s*\(", RegexOptions.Multiline), "Singleton hard / DDOL: governa bene la vita o usa un registry."),
            ("PlayerPrefs_Raw", new Regex(@"PlayerPrefs\.(Set|Get)", RegexOptions.Multiline), "PlayerPrefs raw: incapsula in SaveService.")
        };

        int totalHits = 0;
        foreach (var (key, rx, why) in patterns)
        {
            sb.AppendLine($"## {key}");
            sb.AppendLine($"_Motivo_: {why}");
            int hits = 0;

            foreach (var file in scripts)
            {
                var text = File.ReadAllText(file);
                if (rx.IsMatch(text))
                {
                    hits++;
                    sb.AppendLine($"- {Pretty(file)}");
                }
            }
            totalHits += hits;
            if (hits == 0) sb.AppendLine("- (none)");
            sb.AppendLine();
        }

        // Scene loading anti-patterns
        sb.AppendLine("## Scene_Load_Single (LoadScene senza Additive?)");
        var sceneLoadRx = new Regex(@"SceneManager\.LoadScene\s*\(\s*\"".+?\""\s*\)", RegexOptions.Multiline);
        int sl = 0;
        foreach (var file in scripts)
        {
            var text = File.ReadAllText(file);
            if (sceneLoadRx.IsMatch(text) && !text.Contains("LoadSceneMode.Additive"))
            {
                sl++;
                sb.AppendLine($"- {Pretty(file)}");
            }
        }
        if (sl == 0) sb.AppendLine("- (none)");
        sb.AppendLine();

        // Scenes list
        var scenes = Directory.GetFiles(Path.Combine(Application.dataPath, "Scenes"), "*.unity", SearchOption.AllDirectories);
        sb.AppendLine("## Scenes found");
        if (scenes.Length == 0) sb.AppendLine("- (none)");
        foreach (var s in scenes.OrderBy(x => x)) sb.AppendLine($"- {Pretty(s)}");
        sb.AppendLine();

        File.WriteAllText(outPath, sb.ToString(), Encoding.UTF8);
        EditorUtility.RevealInFinder(Path.GetFullPath(outPath));
        Debug.Log($"[hp55games] StaticScan exported → {outPath}");
    }

    private static string Pretty(string path)
    {
        var p = path.Replace("\\", "/");
        var i = p.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase);
        return i >= 0 ? p.Substring(i + 1) : p;
    }
}

