namespace hp55games.Mobile.Core.Context
{
    /// <summary>
    /// Default implementation of IGameContextService.
    /// Simple in-memory container for the current session state.
    /// </summary>
    public sealed class GameContextService : IGameContextService
    {
        public string ProfileId     { get; set; } = "default";
        public string CurrentLevelId { get; set; }
        public int    CurrentRunSeed { get; set; }
        public bool   IsDebug        { get; set; }

        public void ResetRun()
        {
            CurrentLevelId = null;
            CurrentRunSeed = 0;
        }
    }
}