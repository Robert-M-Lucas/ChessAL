using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public void UpdateFields()
    {
        PlayerIDText.text = PlayerID.ToString();
        PlayerNameText.text = PlayerName;

        if (Team != -1) TeamText.text = $"Team: {(Team+1)}";
        else TeamText.text = "Spectator";

        if (PlayerOnTeam != -1) PlayerOnTeamText.text = $"Player: {(PlayerOnTeam + 1)}";
        else PlayerOnTeamText.text = "Spectator";
    }

    public void OnTeamClick()
    {
        if (!ChessManager.IsHost()) return;

        TeamSize[] team_sizes = ChessManager.CurrentGameManager.GetTeamSizes();

        Team++;
        if (Team >= team_sizes.Length) Team = -1;

        if (Team == -1) PlayerOnTeam = -1;

        ChessManager.HostSetTeam(PlayerID, Team, PlayerOnTeam);
        UpdateFields();
    }

    public void OnPlayerOnTeamClick()
    {
        if (!ChessManager.IsHost()) return;
        if (Team == -1) return;

        TeamSize team_size = ChessManager.CurrentGameManager.GetTeamSizes()[Team];

        PlayerOnTeam++;
        if (PlayerOnTeam >= team_size.Max) PlayerOnTeam = 0;


        ChessManager.HostSetTeam(PlayerID, Team, PlayerOnTeam);
        UpdateFields();
    }
}