using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.KingOfTheHill
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 300;

        public override string GetName() => "King of the Hill";

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : NormalChess.GameManager
    {
        public GameManager(AbstractGameManagerData d, ChessManager chessManager) : base(d, chessManager)
        {
            Board = new NormalChess.Board(this);
        }

        public override int OnMove(V2 from, V2 to)
        {
            int default_return = FalseOnMove(from, to);

            List<V2> centers = new List<V2>() { new V2(3, 3), new V2(4, 3), new V2(3, 4), new V2(4, 4) };

            foreach (V2 cell in centers)
            {
                if (Board.GetPiece(cell) is not null && Board.GetPiece(cell).GetUID() == NormalChess.PieceUIDs.KING)
                {
                    if (Board.GetPiece(cell).Team == 0) return GUtil.TurnEncodeTeam(0);
                    else return GUtil.TurnEncodeTeam(1);
                }
            }

            return default_return;
        }
    }
}
