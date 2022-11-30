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

        public TMP_Text PlayerIDText;
        public TMP_Text PlayerNameText;
        public TMP_Text TeamText;
        public TMP_Text PlayerOnTeamText;

        public void Start()
        {
            
        }

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
            else TeamText.text = "[Click to set team]";

            if (PlayerOnTeam != -1) PlayerOnTeamText.text = $"Player: {(PlayerOnTeam + 1)}";
            else PlayerOnTeamText.text = "[Click to set player number]";
        }

        public void OnTeamClick()
        {
            if (!ChessManager.IsHost() && !ChessManager.localPlay) return;

            TeamSize[] team_sizes = ChessManager.CurrentGameManager.GetTeamSizes();

            Team++;
            if (Team >= team_sizes.Length) Team = -1;

            if (Team == -1)
                PlayerOnTeam = -1;
            else if (PlayerOnTeam == -1)
                PlayerOnTeam = 0;

            ChessManager.HostSetTeam(PlayerID, Team, PlayerOnTeam);
            UpdateFields();
        }

        public void OnPlayerOnTeamClick()
        {
            if (!ChessManager.IsHost() && !ChessManager.localPlay) return;
            if (Team == -1) return;

            TeamSize team_size = ChessManager.CurrentGameManager.GetTeamSizes()[Team];

            PlayerOnTeam++;
            if (PlayerOnTeam >= team_size.Max) PlayerOnTeam = 0;


            ChessManager.HostSetTeam(PlayerID, Team, PlayerOnTeam);
            UpdateFields();
        }
    }
}