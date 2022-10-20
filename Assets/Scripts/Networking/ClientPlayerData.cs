namespace Networking.Client
{
    /// <summary>
    /// Data client holds about other players
    /// </summary>
    public class ClientPlayerData
    {
        public int PlayerID;
        public string Name;

        public int Team;

        /// <summary>
        /// E.g. Player *1* in team x
        /// </summary>
        public int PlayerOnTeam;

        public ClientPlayerData(int playerID, string name, int team, int playerInTeam)
        {
            PlayerID = playerID;
            Name = name;
            Team = team;
            PlayerOnTeam = playerInTeam;
        }
    }
}