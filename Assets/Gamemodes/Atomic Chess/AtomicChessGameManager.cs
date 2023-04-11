using Gamemodes.NormalChess;
using Game;

namespace Gamemodes.AtomicChess
{
    public class GameManagerData : NormalChess.GameManagerData
    {
        public override AbstractGameManager Instantiate(LiveGameData initialData)
        {
            return new GameManager(this);
        }

        public override int GetUID() => 400;

        public override string GetName() => "Atomic Chess";

        public override string GetDescription()
        {
            return @"Atomic Chess

Must have one player on both the black and white team

When a piece is taken a 3x3 area around it is destroyed";
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
            var explode = Board.GetPiece(move.To) is not null; // Was piece taken

            var default_return = FalseOnMove(Board, move, gameData);

            // Removes pieces around explosion
            if (explode)
            {
                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        // if (x == 0 && y == 0) continue;
                        var new_pos = new V2(x, y) + move.To;
                        if (GUtil.IsOnBoard(new_pos, Board)) Board.PieceBoard[new_pos.X, new_pos.Y] = null;
                    }
                }
            }

            var kings_alive = CheckForKings(Board);

            return kings_alive switch
            {
                KingsAlive.None => GUtil.TurnEncodeTeam(gameData.CurrentPlayer),
                KingsAlive.Black => GUtil.TurnEncodeTeam(1),
                KingsAlive.White => GUtil.TurnEncodeTeam(0),
                _ => default_return
            };
        }
    }
}
