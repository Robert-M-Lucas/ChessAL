using System.Collections.Concurrent;

namespace Game
{
    /// <summary>
    /// Represents the current state of a game other than the board
    /// </summary>
    public class LiveGameData
    {
        private ChessManager chessManager;

        public int LocalPlayerTeam;

        public int LocalPlayerID;

        private int currentPlayer;
        public int CurrentPlayer { get { return currentPlayer; } set { currentPlayer = value; UpdateCurrentTeam(); } }

        public int CurrentTeam;

        public LiveGameData(ChessManager chessManager) { this.chessManager = chessManager; }

        public LiveGameData Clone()
        {
            var game_data = new LiveGameData(chessManager);
            game_data.LocalPlayerTeam = LocalPlayerTeam;
            game_data.LocalPlayerID = LocalPlayerID;
            game_data.CurrentPlayer = CurrentPlayer;
            return game_data;
        }

        private void UpdateCurrentTeam()
        {
            CurrentTeam = GetPlayerList()[CurrentPlayer].Team;
        }

        public int? GetPlayerByTeam(int team, int playerInTeam) => chessManager.GetPlayerByTeam(team, playerInTeam);

        public ConcurrentDictionary<int, Networking.Client.ClientPlayerData> GetPlayerList() => chessManager.GetPlayerList();
    }
}