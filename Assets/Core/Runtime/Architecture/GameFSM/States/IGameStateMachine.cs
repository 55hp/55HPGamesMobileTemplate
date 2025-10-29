using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Architecture.States
{
    public interface IGameStateMachine
    {
        IGameState Current { get; }
        Task ChangeStateAsync(IGameState next);
        void CancelCurrent();
    }
}