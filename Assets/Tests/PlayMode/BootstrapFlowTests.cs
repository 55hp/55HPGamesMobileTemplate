using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class BootstrapFlowTests
{
    private const string Bootstrap = "00_Bootstrap";
    private const string Menu      = "01_Menu";
    private const string UiRootGO  = "UIRoot"; // nome GameObject del tuo prefab root UI

    [UnityTest]
    public IEnumerator Bootstrap_Loads_Menu_And_UIRoot_Present()
    {
        // Carica la scena di bootstrap in Single per simulare una avvio "pulito"
        yield return SceneManager.LoadSceneAsync(Bootstrap, LoadSceneMode.Single);

        // Attendi che il Menu sia caricato additive e diventi active scene (timeout di sicurezza)
        const float timeout = 10f;
        float t = 0f;
        while (SceneManager.GetActiveScene().name != Menu && t < timeout)
        {
            t += Time.deltaTime;
            yield return null;
        }
        Assert.AreEqual(Menu, SceneManager.GetActiveScene().name, $"Active scene should be {Menu} within {timeout}s.");

        // Verifica che esista un GameObject chiamato "UIRoot"
        var uiRoot = GameObject.Find(UiRootGO);
        Assert.IsNotNull(uiRoot, "UIRoot GameObject should be present (loaded from 91_UI_Root).");

        // (Facoltativo) verifica che le scene additive siano effettivamente caricate
        Assert.IsTrue(IsLoaded("90_Systems_Audio"), "90_Systems_Audio should be loaded additive.");
        Assert.IsTrue(IsLoaded("91_UI_Root"),       "91_UI_Root should be loaded additive.");
    }

    private bool IsLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
            if (SceneManager.GetSceneAt(i).name == sceneName) return true;
        return false;
    }
}