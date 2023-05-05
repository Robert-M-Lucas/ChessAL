using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamemodes.NormalChess
{
    public class PawnPiece : NormalChessPiece
    {
        /// <summary>
        /// Tracks whether piece has moved. Used for determining whether it can dash
        /// </summary>
        public bool HasMoved = false;

        /// <summary>
        /// Tracks turn on which pawn dashed. Used for en passant
        /// </summary>
        public int DashMove = -1;

        public PawnPiece(V2 position, int team, AbstractBoard board) : base(position, team, board)
        {
            AppearanceID = 100;
            if (team != 0) AppearanceID += 6;
        }

        public override List<Move> GetMoves()
        {
            var forward_moves = new List<Move>();
            var attacking_moves = new List<Move>();
            var en_passant = new List<Move>();
            var m = 1;
            if (Team == 1) m = -1;

            forward_moves.Add(new Move(Position, Position + new V2(0, 1 * m)));

            // En passant logic

            if (GUtil.IsOnBoard(Position + new V2(1, 1 * m), Board) && // Is move to square empty
                Board.GetPiece(Position + new V2(1, 1 * m)) is null)
            {
                if (GUtil.IsOnBoard(Position + new V2(-1, 0), Board) && Board.GetPiece(Position + new V2(1, 0)) is not null) // Is there a piece beside me
                {
                    var piece = Board.GetPiece(Position + new V2(1, 0));
                    if (piece.GetUID() == GetUID() && 
                            ((PawnPiece) piece).DashMove == ((Board) Board).MoveCounter - 1 &&
                            ((PawnPiece) piece).Team != Team) // Is this a pawn, dashed last turn and not on my team
                        en_passant.Add(new Move(Position, Position + new V2(1, 1 * m))); // Add en passant move
                }
            }
            else
            {
                attacking_moves.Add(new Move(Position, Position + new V2(1, 1 * m)));
            }

            // En passant logic for other side

            if (GUtil.IsOnBoard(Position + new V2(-1, 1 * m), Board) &&
                Board.GetPiece(Position + new V2(-1, 1 * m)) is null)
            {
                if (GUtil.IsOnBoard(Position + new V2(-1, 0), Board) && Board.GetPiece(Position + new V2(-1, 0)) is not null)
                {
                    var piece = Board.GetPiece(Position + new V2(-1, 0));
                    if (piece.GetUID() == GetUID() && 
                            ((PawnPiece) piece).DashMove == ((Board) Board).MoveCounter - 1 &&
                            ((PawnPiece) piece).Team != Team) 
                        en_passant.Add(new Move(Position, Position + new V2(-1, 1 * m)));
                }
            }
            else
            {
                attacking_moves.Add(new Move(Position, Position + new V2(-1, 1 * m)));
            }

            // Remove attacking moves that don't attack an enemy piece
            attacking_moves = GUtil.RemoveNonEnemy(GUtil.RemoveBlocked(attacking_moves, Board), Board);

            // Add dash move if pawn hasn't moved yet
            if (!HasMoved && Board.GetPiece(Position + new V2(0, m)) is null)
            {
                forward_moves.Add(new Move(Position, Position + new V2(0, 2 * m)));
            }

            // Remove blocked moves
            forward_moves = GUtil.RemoveNonEmpty(GUtil.RemoveBlocked(forward_moves, Board), Board);

            return forward_moves.Concat(attacking_moves).Concat(en_passant).ToList();
        }

        public override void OnMove(Move move, bool thisPiece)
        {
            if (thisPiece)
            {
                HasMoved = true;
                if (move.To - move.From == new V2(0, 2) || move.To - move.From == new V2(0, -2))
                {
                    DashMove = ((Board) Board).MoveCounter; // Track when piece dashed
                }
                else if ((move.To - move.From).X != 0 && Board.GetPiece(move.To) is null) // En passant
                {
                    // Destroy piece behind pawn
                    if (Team == 0)
                    {
                        Board.PieceBoard[move.To.X, move.To.Y - 1] = null;
                    }
                    else
                    {
                        Board.PieceBoard[move.To.X, move.To.Y + 1] = null;
                    }
                }

                // Piece promotion
                if ((move.To.Y == Board.PieceBoard.GetLength(1) - 1 && Team == 0) || (move.To.Y == 0 && Team == 1))
                {
                    ((GameManager) Board.GameManager).CancelDefaultMove = true; // Cancel normal movement
                    Board.PieceBoard[move.To.X, move.To.Y] = new QueenPiece(move.To, Team, Board);
                    Board.PieceBoard[move.From.X, move.From.Y] = null;
                }
            }

            base.OnMove(move, thisPiece);
        }

        public override AbstractPiece Clone(AbstractBoard newBoard)
        {
            var new_piece = new PawnPiece(Position, Team, newBoard);
            new_piece.DashMove = DashMove;
            new_piece.HasMoved = HasMoved;
            return new_piece;
        }

        public override PieceSerialisationData GetData()
        {
            var data = new PieceSerialisationData();
            data.Team = Team;
            data.Position = Position;
            data.UID = GetUID();

            // Add custom save data
            data.Data = new byte[5];
            data.Data = ArrayExtensions.Merge(bigArr: data.Data, smallArr: BitConverter.GetBytes(HasMoved), index: 0);
            data.Data = ArrayExtensions.Merge(data.Data, BitConverter.GetBytes(DashMove), index: 1);

            return data;
        }

        public override void LoadData(PieceSerialisationData data)
        {
            // Load custom save data
            HasMoved = BitConverter.ToBoolean(ArrayExtensions.Slice(data.Data, 0, 1));
            DashMove = BitConverter.ToInt32(ArrayExtensions.Slice(data.Data, 1, 5));

            base.LoadData(data);
        }

        public override int GetUID() => 100;
    }
}