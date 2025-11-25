using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.InputSystem;

public sealed class InputServiceDriver : MonoBehaviour
{
    private IInputService _input;

    private void Awake()
    {
        _input = ServiceRegistry.Resolve<IInputService>();
    }

    private void Update()
    {
        _input?.Tick(Time.unscaledDeltaTime);
    }
}