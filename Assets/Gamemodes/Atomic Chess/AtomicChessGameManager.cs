using Gamemodes.NormalChess;
using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Gamemodes.AtomicChess
{
    public class GameManagerData : NormalChess.GameManagerData
    {
        public override AbstractGameManager Instantiate()
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
            Board = new NormalChess.Board(this);
        }

        public override int OnMove(Move move, LiveGameData gameData)
        {
            bool explode = Board.GetPiece(move.To) is not null;

            int default_return = FalseOnMove(Board, move, gameData);

            // Removes pieces around explosion
            if (explode)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        // if (x == 0 && y == 0) continue;
                        V2 new_pos = new V2(x, y) + move.To;
                        if (GUtil.IsOnBoard(new_pos, Board)) Board.PieceBoard[new_pos.X, new_pos.Y] = null;
                    }
                }
            }

            KingAlive kings_alive = CheckForKings(Board);

            if (kings_alive == KingAlive.None) return GUtil.TurnEncodeTeam(gameData.CurrentPlayer);
            if (kings_alive == KingAlive.Black) return GUtil.TurnEncodeTeam(1);
            if (kings_alive == KingAlive.White) return GUtil.TurnEncodeTeam(0);

            return default_return;
        }
    }
}
