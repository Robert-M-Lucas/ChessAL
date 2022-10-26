using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.MegaChess
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 500;

        public override string GetName() => "Mega Chess";

        public override string GetDescription()
        {
            return @"Mega Chess

Must have 2 teams of 1

Chess played on a 16x16 board with more pieces on each side";
        }

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

