using System.Threading;
using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Architecture.States
{
    public interface IGameState
    {
        /// <summary>Chiamato quando lo stato entra in esecuzione.</summary>
        Task EnterAsync(CancellationToken ct);

        /// <summary>Chiamato quando lo stato esce (rilascia risorse, salva, ecc.).</summary>
        Task ExitAsync(CancellationToken ct);
    }
}