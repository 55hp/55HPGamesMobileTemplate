using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using hp55games.Mobile.Core.Config; // per GameConfig

public class ConfigSmokeTest : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadAndLog());
    }

    private IEnumerator LoadAndLog()
    {
        AsyncOperationHandle<GameConfig> handle = Addressables.LoadAssetAsync<GameConfig>("config/main");
        yield return handle; // aspetta il caricamento

        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
        {
            Debug.LogError("[ConfigSmokeTest] Fallito il load di config/main");
            yield break;
        }

        var cfg = handle.Result;
        Debug.Log($"[ConfigSmokeTest] AppVersion={cfg.appVersion} | Haptics={cfg.enableHaptics} | DefaultDifficulty={cfg.defaultDifficulty}");

        // Per lo smoke test non rilasciamo: resta in memoria fino a fine play.
        // Se volessi rilasciare: Addressables.Release(handle);
    }
}