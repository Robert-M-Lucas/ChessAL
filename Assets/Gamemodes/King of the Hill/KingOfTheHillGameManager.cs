using System.Collections.Generic;
using System.Linq;
using Game;

namespace Gamemodes.KingOfTheHill
{
    public class GameManagerData : NormalChess.GameManagerData
    {
        public override AbstractGameManager Instantiate(LiveGameData initialData)
        {
            return new GameManager(this);
        }

        public override int GetUID() => 300;

        public override string GetName() => "King of the Hill";

        public override string GetDescription()
        {
            return @"King of the Hill

Must have one player on both the black and white team

First king to the 2x2 square in the center of the board wins. Normal check and checkmate rules apply";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : NormalChess.GameManager
    {
        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new Board(this);
        }

        public override int OnMove(Move move, LiveGameData gameData)
        {
            var default_return = FalseOnMove(Board, move, gameData); // Get normal return value from NormalChess

            // Check for centre winner
            var centers = new List<V2>() { new V2(3, 3), new V2(4, 3), new V2(3, 4), new V2(4, 4) };

            foreach (var cell in centers.Where(cell => Board.GetPiece(cell) is not null && Board.GetPiece(cell).GetUID() == NormalChess.PieceUIDs.KING))
            {
                return GUtil.TurnEncodeTeam(Board.GetPiece(cell).Team == 0 ? 0 : 1);
            }

            // Else return default
            return default_return;
        }
    }
}
