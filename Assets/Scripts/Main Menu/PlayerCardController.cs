using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MainMenu
{
    public class PlayerCardController : MonoBehaviour
    {
        public int PlayerID;
        public string PlayerName;
        public int Team;
        public int PlayerOnTeam;

        public ChessManager ChessManager;
        public MenuUIManager MenuUIManager;

        [SerializeField] private TMP_Text PlayerIDText;
        [SerializeField] private TMP_Text PlayerNameText;
        [SerializeField] private TMP_Text TeamText;
        [SerializeField] private TMP_Text PlayerOnTeamText;

        /// <summary>
        /// Updates the text on player cards
        /// </summary>
        public void UpdateFields()
        {
            PlayerIDText.text = PlayerID.ToString();
            PlayerNameText.text = PlayerName;

            if (Team != -1)
            {
                if (ChessManager.CurrentGameManager.TeamAliases().Length > 0)
                {
                    TeamText.text = ChessManager.CurrentGameManager.TeamAliases()[Team];
                }
                else TeamText.text = $"Team: {(Team + 1)}";
            }
            else
            {
                if (ChessManager.IsHost() || ChessManager.LocalPlay)
                    TeamText.text = "[Click to set team]";
                else
                    TeamText.text = "[Unset]";
            }

            if (PlayerOnTeam != -1) PlayerOnTeamText.text = $"Player: {(PlayerOnTeam + 1)}";
            else
            {
                if (ChessManager.IsHost() || ChessManager.LocalPlay)
                    PlayerOnTeamText.text = "[Unset]";
                else
                    PlayerOnTeamText.text = "[Unset]";
            }
        }

        public void OnTeamClick()
        {
            if (!ChessManager.IsHost() && !ChessManager.LocalPlay) return; // Player is not host

            TeamSize[] team_sizes = ChessManager.CurrentGameManager.GetTeamSizes();

            Team = ChessManager.FindNextNonFullTeam(Team, team_sizes);

            if (Team == -1) PlayerOnTeam = -1;
            else PlayerOnTeam = 0;

            ChessManager.HostSetTeam(PlayerID, Team, PlayerOnTeam);
            UpdateFields();
        }

        public void OnPlayerOnTeamClick()
        {
            if (!ChessManager.IsHost() && !ChessManager.LocalPlay) return; // Player is not host
            if (Team == -1) return; // Player is spectator

            TeamSize team_size = ChessManager.CurrentGameManager.GetTeamSizes()[Team];

            PlayerOnTeam++;
            if (PlayerOnTeam >= team_size.Max) PlayerOnTeam = 0;


            ChessManager.HostSetTeam(PlayerID, Team, PlayerOnTeam);
            UpdateFields();
        }
    }
}