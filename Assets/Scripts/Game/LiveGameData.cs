using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

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

        private int _CurrentPlayer;
        public int CurrentPlayer { get { return _CurrentPlayer; } set { _CurrentPlayer = value; UpdateCurrentTeam(); } }

        public int CurrentTeam;

        public LiveGameData(ChessManager chessManager) { this.chessManager = chessManager; }

        public LiveGameData Clone()
        {
            LiveGameData gameData = new LiveGameData(chessManager);
            gameData.LocalPlayerTeam = LocalPlayerTeam;
            gameData.LocalPlayerID = LocalPlayerID;
            gameData.CurrentPlayer = CurrentPlayer;
            return gameData;
        }

        private void UpdateCurrentTeam()
        {
            CurrentTeam = GetPlayerList()[CurrentPlayer].Team;
        }

        public int? GetPlayerByTeam(int team, int playerInTeam) => chessManager.GetPlayerByTeam(team, playerInTeam);

        public ConcurrentDictionary<int, Networking.Client.ClientPlayerData> GetPlayerList() => chessManager.GetPlayerList();
    }
}