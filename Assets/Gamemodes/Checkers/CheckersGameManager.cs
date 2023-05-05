using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game;

namespace Gamemodes.Checkers
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(LiveGameData initialData)
        {
            return new GameManager(this);
        }
        public override int GetUID() => 700;
        public override string GetName() => "Checkers";
        public override string GetDescription()
        {
            return @"Checkers

Must have one player on both the black and white team

Traditional checkers played on an 8x8 board";
        }
        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
        public override string[] TeamAliases() => new string[] { "Black", "White" };
    }

    public class GameManager : AbstractGameManager
    {
        // Tracks whether a piece was taken to allow double takes
        public bool PieceTaken;

        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new Board(this);
        }

        public List<Move> GetMoves(LiveGameData gameData, V2? enforceFrom = null)
        {
            var moves = Board.GetMoves(gameData);
            // Force move to be by a piece (when chaining jumps)
            if (enforceFrom is not null)
            {
                var i = 0;
                while (i < moves.Count)
                {
                    if (moves[i].From != enforceFrom)
                    {
                        moves.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            // Check if one of the moves is a take
            var can_take = moves.Any(move => (move.To - move.From).X == 2 || (move.To - move.From).X == -2);

            // If can take remove all normal moves
            if (!can_take) return moves;
            {
                var i = 0;
                while (i < moves.Count)
                {
                    if ((moves[i].To - moves[i].From).X != 2 && (moves[i].To - moves[i].From).X != -2)
                    {
                        moves.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            return moves;
        }

        public override List<Move> GetMoves(LiveGameData gameData, bool fastMode)
        {
            return GetMoves(gameData);
        }

        public override int OnMove(Move move, LiveGameData gameData)
        {
            PieceTaken = false;
            var base_return = base.OnMove(move, gameData);

            Board.PieceBoard[move.To.X, move.To.Y] = Board.GetPiece(move.From);
            Board.PieceBoard[move.To.X, move.To.Y].Position = move.To;
            Board.PieceBoard[move.From.X, move.From.Y] = null;

            // If piece was taken and double take available make it the same player's turn again
            if (!PieceTaken) return base_return;
            var next_moves = GetMoves(gameData, move.To);
            if (next_moves.Count > 0 && Mathf.Abs((next_moves[0].To - next_moves[0].From).X) == 2) return gameData.LocalPlayerID;

            return base_return;
        }

        public override AbstractGameManager Clone()
        {
            var new_game_manager = new GameManager(GameManagerData);
            new_game_manager.Board = ((Board) Board).Clone(new_game_manager);

            return new_game_manager;
        }
    }
}