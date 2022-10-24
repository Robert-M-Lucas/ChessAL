using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.Checkers
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 700;

        public override string GetName() => "Checkers";

        public override string GetDescription()
        {
            return @"Checkers
Traditional checkers played on an 8x8 board";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : AbstractGameManager
    {
        
        public bool PieceTaken;

        public GameManager(AbstractGameManagerData d, ChessManager chessManager) : base(d, chessManager)
        {
            Board = new Board(this);
        }

        public override int OnMove(V2 from, V2 to)
        {
            PieceTaken = false;
            int base_return = base.OnMove(from, to);

            Board.PieceBoard[to.X, to.Y] = Board.PieceBoard[from.X, from.Y];
            Board.PieceBoard[to.X, to.Y].Position = to;
            Board.PieceBoard[from.X, from.Y] = null;

            if (PieceTaken) return chessManager.GetLocalPlayerID();
            return base_return;
        }
    }
}