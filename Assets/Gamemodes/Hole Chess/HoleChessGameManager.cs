using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.HoleChess
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 200;

        public override string GetName() => "Hole Chess";

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : NormalChess.GameManager 
    {
        public GameManager(AbstractGameManagerData d, ChessManager chessManager) : base(d, chessManager)
        {
            Board = new Board(this);
        }
    }

}

