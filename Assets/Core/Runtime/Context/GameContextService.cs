namespace hp55games.Mobile.Core.Context
{
    /// <summary>
    /// Default implementation of IGameContextService.
    /// Simple in-memory container for the current session state.
    /// </summary>
    public sealed class GameContextService : IGameContextService
    {
        public string ProfileId      { get; set; } = "default";
        public string CurrentLevelId { get; set; }
        public int    CurrentRunSeed { get; set; }
        public bool   IsDebug        { get; set; }

        public int Score { get; set; }
        public int Lives { get; set; } = -1; // -1 = "no lives system" by default

        public void ResetRun()
        {
            CurrentLevelId = null;
            CurrentRunSeed = 0;
            Score          = 0;
            Lives          = -1;
        }
    }
}