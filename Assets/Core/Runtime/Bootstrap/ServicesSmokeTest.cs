// Assets/Core/Runtime/Bootstrap/ServicesSmokeTest.cs
using UnityEngine;
using AF55HP.Mobile.Core.Architecture;

namespace AF55HP.Mobile.Core.Bootstrap
{
    public class ServicesSmokeTest : MonoBehaviour
    {
        void Start()
        {
            // Log
            var log = ServiceRegistry.Resolve<ILog>();
            log.Info("[SmokeTest] Log ok");

            // Config
            var cfg = ServiceRegistry.Resolve<IConfigService>().Current;
            log.Info($"[SmokeTest] Config ok — version: {cfg.appVersion}");

            // Save
            var save = ServiceRegistry.Resolve<ISaveService>();
            save.Load();
            save.Data.coins += 1;
            save.Save();
            log.Info($"[SmokeTest] Save ok — coins: {save.Data.coins}");

            // EventBus
            var bus = ServiceRegistry.Resolve<IEventBus>();
            var sub = bus.Subscribe<DummyEvent>(e => log.Info($"[SmokeTest] Event ok — msg: {e.Message}"));
            bus.Publish(new DummyEvent { Message = "Hello Bus" });
            sub.Dispose();

            // UI
            var ui = ServiceRegistry.Resolve<IUIService>();
            ui.Toast("UIService ok");
        }
    }

    public struct DummyEvent : IEvent { public string Message; }
}