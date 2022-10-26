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

Must have 2 teams of 1

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

        public List<Move> GetMoves(V2? enforce_from = null)
        {
            List<Move> moves = Board.GetMoves();
            if (enforce_from is not null)
            {
                int i = 0;
                while (i < moves.Count)
                {
                    if (moves[i].From != enforce_from)
                    {
                        moves.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            bool can_take = false;
            foreach (Move move in moves)
            {
                if ((move.To - move.From).X == 2 || (move.To - move.From).X == -2)
                {
                    can_take = true;
                    break;
                }
            }

            if (can_take)
            {
                int i = 0;
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

        public override List<Move> GetMoves()
        {
            return GetMoves(null);
        }

        public override int OnMove(V2 from, V2 to)
        {
            PieceTaken = false;
            int base_return = base.OnMove(from, to);

            Board.PieceBoard[to.X, to.Y] = Board.PieceBoard[from.X, from.Y];
            Board.PieceBoard[to.X, to.Y].Position = to;
            Board.PieceBoard[from.X, from.Y] = null;

            if (PieceTaken) {
                List<Move> next_moves = GetMoves(to);
                if (next_moves.Count > 0 && Mathf.Abs((next_moves[0].To - next_moves[0].From).X) == 2) return chessManager.GetLocalPlayerID();
            }

            return base_return;
        }
    }
}