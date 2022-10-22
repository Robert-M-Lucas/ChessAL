using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 100;

        public override string GetName() => "Normal Chess";

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : AbstractGameManager
    {
        public int MoveCounter;
        public bool CancelDefaultMove;

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

        public override int OnNoMoves()
        {
            return GUtil.TurnEncodeTeam(GUtil.SwitchPlayerTeam(chessManager));
        }

        public override int OnMove(V2 from, V2 to)
        {
            Debug.Log("On move manager");
            CancelDefaultMove = false;

            base.OnMove(from, to);

            if (!CancelDefaultMove)
            {
                Board.PieceBoard[to.X, to.Y] = Board.PieceBoard[from.X, from.Y];
                Board.PieceBoard[to.X, to.Y].Position = to;
                Board.PieceBoard[from.X, from.Y] = null;
            }

            MoveCounter ++;

            bool white_king = false;
            bool black_king = false;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (Board.PieceBoard[x, y] is not null)
                    {
                        if (Board.PieceBoard[x, y].GetUID() == PieceUIDs.KING)
                        {
                            if (Board.PieceBoard[x, y].Team == 0) white_king = true;
                            else black_king = true;
                        }
                    }
                }
            }

            if (!white_king) return GUtil.TurnEncodeTeam(1);
            if (!black_king) return GUtil.TurnEncodeTeam(0);

            return GUtil.SwitchPlayerTeam(chessManager);
        }
    }
}