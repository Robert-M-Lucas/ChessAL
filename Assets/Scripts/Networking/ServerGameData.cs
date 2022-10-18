using System.IO;
using Gamemodes;

#nullable enable

namespace Networking.Server
{
    /// <summary>
    /// Holds data about a server
    /// </summary>
    public class ServerGameData
    {
        public int GameModeID;

        public TeamSize[] TeamSizes;

        public byte[] SaveData;

        public ServerGameData(AbstractGameManagerData gameManager, string? savePath)
        {
            if (savePath is not null) SaveData = File.ReadAllBytes(savePath);
            else SaveData = new byte[0];

            GameModeID = gameManager.GetUID();

            TeamSizes = gameManager.GetTeamSizes();
        }
    }
}