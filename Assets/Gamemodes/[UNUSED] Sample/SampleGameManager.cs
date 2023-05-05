

/*

namespace Gamemodes.Sample
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 1;

        public override string GetName() => "Sample";

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : AbstractGameManager
    {
        public GameManager(AbstractGameManagerData d, ChessManager chessManager) : base(d, chessManager)
        {
            Board = new Board(this);
        }

        public override void LoadData(byte[] data)
        {

        }

        public override List<Move> GetMoves()
        {
            return Board.GetMoves();
        }

        public override int OnMove(V2 from, V2 to)
        {
            Debug.Log("On move manager");
            Board.PieceBoard[to.X, to.Y] = Board.PieceBoard[from.X, from.Y];
            Board.PieceBoard[to.X, to.Y].Position = to;
            Board.PieceBoard[from.X, from.Y] = null;

            if (Random.Range(0, 100) > 10)
            {
                if (chessManager.GetLocalPlayerID() == 0) return 1;
                return 0;
            }
            else
            {
                return GUtil.TurnEncodeTeam((int) Mathf.Floor(Random.Range(0, 1.999f)));
            }
        }
    }
}

*/