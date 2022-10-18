using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.Sample
{
    public class SampleGameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new SampleGameManager(this, chessManager);
        }

        public override int GetUID() => 1;

        public override string GetName() => "Sample Gamemode";

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 2) };
    }

    public class SampleGameManager : AbstractGameManager
    {
        public SampleGameManager(AbstractGameManagerData d, ChessManager chessManager) : base(d, chessManager)
        {
            Board = new SampleBoard();
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
            Board.PieceBoard[to.X, to.Y] = Board.PieceBoard[from.X, from.Y];
            Board.PieceBoard[to.X, to.Y].Position = to;
            Board.PieceBoard[from.X, from.Y] = null;

            if (chessManager.GetLocalPlayerID() == 0) return 1;
            return 0;
        }
    }
}