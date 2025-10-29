using System;
using System.Collections.Generic;

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
            Register<ISaveService>(new SaveService());
            Register<IContentLoader>(new AddressablesContentLoader());
            Register<IUIService>(new UIService());
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