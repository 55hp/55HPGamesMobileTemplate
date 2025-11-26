using System.Threading.Tasks;

namespace hp55games.Mobile.Core.SceneFlow
{
    public interface ISceneFlowService
    {
        Task GoToMenuAsync();
        Task GoToGameplayAsync(string levelId = null);
        Task GoToResultsAsync();
        Task GoToPauseAsync();
    }
}

