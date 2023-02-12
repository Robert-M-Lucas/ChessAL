using System.Collections.Generic;
using UnityEngine;
using Game;

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

        public override List<Move> GetMoves(LiveGameData gameData, bool fastMode)
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

            // Horizontal
            for (int y = 0; y < 9; y++)
            {
                int team = -1;
                int count = 0;

                for (int x = 0; x < 7; x++)
                {
                    if (Board.PieceBoard[x, y] is null)
                    {
                        team = -1;
                        count = 0;
                    }
                    else
                    {
                        if (team == Board.PieceBoard[x, y].Team) count++;
                        else 
                        {
                            team = Board.PieceBoard[x, y].Team;
                            count = 1;
                        }
                    }

                    if (count >= 4) return GUtil.TurnEncodeTeam(team);
                }
            }

            // Vertical
            for (int x = 0; x < 7; x++)
            {
                int team = -1;
                int count = 0;

                for (int y = 0;y < 9; y++)
                {
                    if (Board.PieceBoard[x, y] is null)
                    {
                        team = -1;
                        count = 0;
                    }
                    else
                    {
                        if (team == Board.PieceBoard[x, y].Team) count++;
                        else
                        {
                            team = Board.PieceBoard[x, y].Team;
                            count = 1;
                        }
                    }

                    if (count >= 4) return GUtil.TurnEncodeTeam(team);
                }
            }

            // Diagonal / pt.1
            for (int y = 0; y < 9; y++)
            {
                int team = -1;
                int count = 0;

                for (int x = 0; x < 7 && x < (9 - y); x++)
                {
                    if (Board.PieceBoard[x, y + x] is null)
                    {
                        team = -1;
                        count = 0;
                    }
                    else
                    {
                        if (team == Board.PieceBoard[x, y + x].Team) count++;
                        else
                        {
                            team = Board.PieceBoard[x, y + x].Team;
                            count = 1;
                        }
                    }

                    if (count >= 4) return GUtil.TurnEncodeTeam(team);
                }
            }

            // Diagonal / pt.2
            for (int x = 1; x < 7; x++)
            {
                int team = -1;
                int count = 0;

                for (int y = 0; y < 9 && y < (7 - x); y++)
                {
                    if (Board.PieceBoard[x + y, y] is null)
                    {
                        team = -1;
                        count = 0;
                    }
                    else
                    {
                        if (team == Board.PieceBoard[x + y, y].Team) count++;
                        else
                        {
                            team = Board.PieceBoard[x + y, y].Team;
                            count = 1;
                        }
                    }

                    if (count >= 4) return GUtil.TurnEncodeTeam(team);
                }
            }

            // Diagonal \ pt.1
            for (int y = 0; y < 9; y++)
            {
                int team = -1;
                int count = 0;

                for (int x = 6; x >= 0 && y + (6 - x) < 9; x--)
                {
                    if (Board.PieceBoard[x, y + (6 - x)] is null)
                    {
                        team = -1;
                        count = 0;
                    }
                    else
                    {
                        if (team == Board.PieceBoard[x, y + (6 - x)].Team) count++;
                        else
                        {
                            team = Board.PieceBoard[x, y + (6 - x)].Team;
                            count = 1;
                        }
                    }

                    if (count >= 4) return GUtil.TurnEncodeTeam(team);
                }
            }

            // Diagonal \ pt.2
            for (int x = 6; x >= 0; x--)
            {
                int team = -1;
                int count = 0;

                for (int y = 0; y < 9 && x - y >= 0; y++)
                {
                    if (Board.PieceBoard[x - y, y] is null)
                    {
                        team = -1;
                        count = 0;
                    }
                    else
                    {
                        if (team == Board.PieceBoard[x - y, y].Team) count++;
                        else
                        {
                            team = Board.PieceBoard[x - y, y].Team;
                            count = 1;
                        }
                    }

                    if (count >= 4) return GUtil.TurnEncodeTeam(team);
                }
            }

            return 0; // No win
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