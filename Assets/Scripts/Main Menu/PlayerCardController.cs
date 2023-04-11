using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("PlayerIDText")] [SerializeField] private TMP_Text playerIDText;
        [FormerlySerializedAs("PlayerNameText")] [SerializeField] private TMP_Text playerNameText;
        [FormerlySerializedAs("TeamText")] [SerializeField] private TMP_Text teamText;
        [FormerlySerializedAs("PlayerOnTeamText")] [SerializeField] private TMP_Text playerOnTeamText;

        /// <summary>
        /// Updates the text on player cards
        /// </summary>
        public void UpdateFields()
        {
            playerIDText.text = PlayerID.ToString();
            playerNameText.text = PlayerName;

            if (Team != -1)
            {
                if (ChessManager.CurrentGameManager.TeamAliases().Length > 0)
                {
                    teamText.text = ChessManager.CurrentGameManager.TeamAliases()[Team];
                }
                else teamText.text = $"Team: {(Team + 1)}";
            }
            else
            {
                if (ChessManager.IsHost() || ChessManager.LocalPlay)
                    teamText.text = "[Click to set team]";
                else
                    teamText.text = "[Unset]";
            }

            if (PlayerOnTeam != -1) playerOnTeamText.text = $"Player: {(PlayerOnTeam + 1)}";
            else
            {
                if (ChessManager.IsHost() || ChessManager.LocalPlay)
                    playerOnTeamText.text = "[Unset]";
                else
                    playerOnTeamText.text = "[Unset]";
            }
        }

        public void OnTeamClick()
        {
            if (!ChessManager.IsHost() && !ChessManager.LocalPlay) return; // Player is not host

            var team_sizes = ChessManager.CurrentGameManager.GetTeamSizes();

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

            var team_size = ChessManager.CurrentGameManager.GetTeamSizes()[Team];

            PlayerOnTeam++;
            if (PlayerOnTeam >= team_size.Max) PlayerOnTeam = 0;


            ChessManager.HostSetTeam(PlayerID, Team, PlayerOnTeam);
            UpdateFields();
        }
    }
}