using NUnit.Framework;
using System;
using System.Linq;

public class CorePresenceTests
{
    // Nomi fully-qualified che vogliamo ci siano nel template
    private static readonly string[] ExpectedTypes =
    {
        "hp55games.Ui.UiManager",
        "hp55games.Core.EventBus",
        "hp55games.Core.Logger",
        "hp55games.Config.ConfigService",
        // Se usi il tuo naming:
        "hp55games.Mobile.Core.Architecture.UnityLog",
        "hp55games.Mobile.Core.Architecture.EventBus",
        "hp55games.Mobile.Core.Architecture.ConfigService",
        "hp55games.Mobile.Core.SaveService",
        "hp55games.Services.SaveService"
    };

    [Test]
    public void UnityVersion_IsAvailable()
    {
        Assert.IsFalse(string.IsNullOrEmpty(UnityEngine.Application.unityVersion), "Unity version should not be empty.");
    }

    [Test]
    public void Core_Types_Are_Present_ByName()
    {
        var asms = AppDomain.CurrentDomain.GetAssemblies();
        bool AnyType(string fqn) =>
            asms.SelectMany(SafeGetTypes).Any(t => t.FullName == fqn);

        var missing = ExpectedTypes.Where(fqn => !AnyType(fqn)).ToArray();

        // Non falliamo per forza se mancano alias secondari; falliamo se mancano TUTTI i principali:
        bool atLeastOneOfEachRole =
            AnyType("hp55games.Ui.UiManager") &&
            (AnyType("hp55games.Core.EventBus") || AnyType("hp55games.Mobile.Core.Architecture.EventBus")) &&
            (AnyType("hp55games.Core.Logger")   || AnyType("hp55games.Mobile.Core.Architecture.UnityLog")) &&
            (AnyType("hp55games.Config.ConfigService") || AnyType("hp55games.Mobile.Core.Architecture.ConfigService")) &&
            (AnyType("hp55games.Services.SaveService") || AnyType("hp55games.Mobile.Core.SaveService"));

        Assert.IsTrue(atLeastOneOfEachRole,
            "At least one implementation per role (UiManager/EventBus/Logger/Config/Save) must be discoverable by name.\nMissing (informative):\n" + string.Join("\n", missing));
    }

    private static Type[] SafeGetTypes(System.Reflection.Assembly a)
    {
        try { return a.GetTypes(); }
        catch { return Array.Empty<Type>(); }
    }
}
