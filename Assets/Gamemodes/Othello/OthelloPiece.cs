using System.Collections.Generic;

namespace Gamemodes.Othello
{
    public class OthelloPiece : AbstractPiece
    {
        public OthelloPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            Board = board as Board;

            switch (team)
            {
                case 0:
                    AppearanceID = 701;
                    break;
                case 1:
                    AppearanceID = 700;
                    break;
                case 2:
                    AppearanceID = 900;
                    break;
                case 3:
                    AppearanceID = 901;
                    break;
            }
        }

        public override List<Move> GetMoves()
        {
            var all_moves = new List<Move>();

            var directions = new V2[] { 
                V2.Up, V2.Down, V2.Left, V2.Right, 
                V2.Up + V2.Left,
                V2.Down + V2.Left, 
                V2.Up + V2.Right, 
                V2.Down + V2.Right, 
            };

            foreach (var d in directions) {
                var curent_pos = Position + d;

                if (!GUtil.IsOnBoard(curent_pos, Board)) continue;
                var first_piece = Board.GetPiece(curent_pos);
                if (first_piece is null || first_piece.Team == Team) continue;

                while (true)
                {
                    curent_pos += d;
                    if (!GUtil.IsOnBoard(curent_pos, Board)) break;
                    var current_piece = Board.GetPiece(curent_pos);
                    if (current_piece is null) { all_moves.Add(new Move(Position, curent_pos)); break; }
                    if (current_piece.Team == Team) { break; }
                }
            }

            return all_moves;
        }

        public override void OnMove(Move move, bool thisPiece)
        {
            if (!thisPiece) return;

            var directions = new V2[] {
                V2.Up, V2.Down, V2.Left, V2.Right,
                V2.Up + V2.Left,
                V2.Down + V2.Left,
                V2.Up + V2.Right,
                V2.Down + V2.Right,
            };

            Board.PieceBoard[move.To.X, move.To.Y] = new OthelloPiece(move.To, Team, Board);

            void OverwriteLine(V2 start, V2 delta, int count)
            {
                var current = start;
                for (var _ = 0; _ < count; _++)
                {
                    Board.PieceBoard[current.X, current.Y] = new OthelloPiece(current, Team, Board);
                    current += delta;
                }
            }

            foreach (var d in directions)
            {
                var curent_pos = move.To;

                var found = false;
                var count = 0;

                while (true)
                {
                    curent_pos += d;
                    count++;
                    if (!GUtil.IsOnBoard(curent_pos, Board)) break;
                    var current_piece = Board.GetPiece(curent_pos);
                    if (current_piece is null) break;
                    if (current_piece.Team != Team) continue;
                    found = true; break;
                }

                if (found) OverwriteLine(move.To + d, d, count);
            }
        }

        public override int GetUID() => 1000;

        public override AbstractPiece Clone(AbstractBoard newBoard)
        {
            return new OthelloPiece(this.Position, this.Team, newBoard);
        }

        public override float GetValue()
        {
            return 1f;
        }
    }
}