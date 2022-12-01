using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.ConnectFour
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate()
        {
            return new GameManager(this);
        }

        public override int GetUID() => 900;

        public override string GetName() => "Connect Four";

        public override string GetDescription()
        {
            return @"Connect Four

Drop counters onto piles. First person to get four in a row wins

Requires 2 teams";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };

        public override string[] TeamAliases() => new string[] { "Red", "Yellow" };
    }

    public class GameManager : AbstractGameManager
    {
        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new Board(this);
        }

        public override List<Move> GetMoves(LiveGameData gameData)
        {
            return Board.GetMoves(gameData);
        }

        private int CheckForWin(LiveGameData gameData)
        {
            bool full = true;
            for (int x = 0; x < 7; x++)
            {
                if (Board.GetPiece(new V2(x, 8)) is null) { full = false; break; }
            }
            if (full) return GUtil.TurnEncodeTeam(GUtil.SwitchTeam(gameData));

            for (int x = 0; x <= (6 - 3); x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    if (Board.PieceBoard[x, y] is null) continue;

                    int team = Board.PieceBoard[x, y].Team;

                    bool failed = false;
                    for (int x2 = 1; x2 < 4; x2++)
                    {
                        if (Board.PieceBoard[x+x2, y] is null) { failed = true; break; }
                        if (Board.PieceBoard[x+x2, y].Team != team) { failed = true; break; }
                    }

                    if (failed) continue;

                    return GUtil.TurnEncodeTeam(team);
                }
            }

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < (9 - 3); y++)
                {
                    if (Board.PieceBoard[x, y] is null) continue;

                    int team = Board.PieceBoard[x, y].Team;

                    bool failed = false;
                    for (int y2 = 1; y2 < 4; y2++)
                    {
                        if (Board.PieceBoard[x, y + y2] is null) { failed = true; break; }
                        if (Board.PieceBoard[x, y + y2].Team != team) { failed = true; break; }
                    }

                    if (failed) continue;

                    return GUtil.TurnEncodeTeam(team);
                }
            }

            return 0;
        }

        public override int OnMove(Move move, LiveGameData gameData)
        {
            Board.OnMove(move);

            int winner = CheckForWin(gameData);
            if (winner >= 0) return GUtil.SwitchPlayerTeam(gameData);
            else return winner;
        }

        public override AbstractGameManager Clone()
        {
            GameManager new_game_manager = new GameManager(GameManagerData);
            new_game_manager.Board = (Board as Board).Clone(new_game_manager);

            return new_game_manager;
        }

        public override float GetScore(LiveGameData gameData) => 0f;
    }
}