using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.HoleChess
{
    public class GameManagerData : NormalChess.GameManagerData
    {
        public override AbstractGameManager Instantiate(LiveGameData initialData)
        {
            return new GameManager(this);
        }

        public override int GetUID() => 200;

        public override string GetName() => "Hole Chess";

        public override string GetDescription()
        {
            return @"Hole Chess

Must have one player on both the black and white team

Chess with a 2x2 square taken out of the center";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : NormalChess.GameManager 
    {
        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new Board(this);
        }
    }

}

