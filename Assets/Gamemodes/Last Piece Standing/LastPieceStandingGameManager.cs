using Gamemodes.NormalChess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.LastPieceStanding
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 600;

        public override string GetName() => "Last Piece Standing";

        public override string GetDescription()
        {
            return @"Last Piece Standing
Chess played until one team has no pieces remaining";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : NormalChess.GameManager
    {
        public GameManager(AbstractGameManagerData d, ChessManager chessManager) : base(d, chessManager)
        {
            Board = new NormalChess.Board(this);
        }

        public override List<Move> GetMoves()
        {
            (Board as Board).VirtualTeam = chessManager.GetLocalPlayerTeam();
            return Board.GetMoves();
        }

        public override int OnMove(V2 from, V2 to)
        {
            CancelDefaultMove = false;

            Board.OnMove(from, to);

            if (!CancelDefaultMove)
            {
                Board.PieceBoard[to.X, to.Y] = Board.PieceBoard[from.X, from.Y];
                Board.PieceBoard[to.X, to.Y].Position = to;
                Board.PieceBoard[from.X, from.Y] = null;
            }

            (Board as Board).MoveCounter++;

            return GUtil.SwitchTeam(chessManager);
        }
    }
}

