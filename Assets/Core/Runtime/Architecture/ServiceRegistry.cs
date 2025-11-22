using System;
using System.Collections.Generic;
using hp55games.Mobile.Core.Architecture.States;
using hp55games.Mobile.Core.Localization;
using hp55games.Mobile.Core.Pooling;
using hp55games.Mobile.Core.UI;
using hp55games.Mobile.Core.Time;

namespace hp55games.Mobile.Core.Architecture
{
    public static class ServiceRegistry
    {
        static readonly Dictionary<Type, object> _map = new();

        public static void InstallDefaults()
        {
            Register<ILog>(new UnityLog());
            Register<IEventBus>(new EventBus());
            Register<IConfigService>(new ConfigService());
            
            var save = new SaveService();
            save.Load();
            Register<ISaveService>(save);
            
            Register<ITimeService>(new TimeService());
            
            Register<IContentLoader>(new AddressablesContentLoader());
            Register<IGameStateMachine>(new GameStateMachine());
            Register<IObjectPoolService>(new ObjectPoolService());
            Register<IUIOptionsService>(new UIOptionsService());
            Register<ILocalizationService>(new LocalizationService());
        }


        public static void Register<T>(T instance) => _map[typeof(T)] = instance!;
        public static T Resolve<T>() => (T)_map[typeof(T)];

        public static bool TryResolve<T>(out T service)
        {
            if (_map.TryGetValue(typeof(T), out var o))
            {
                service = (T)o;
                return true;
            }

            service = default!;
            return false;
        }
    }
}