using System.Collections.Generic;
using System.Linq;
using Game;

namespace Gamemodes.NormalChess
{
    /// <summary>
    /// Game Manager Data for Normal Chess
    /// </summary>
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(LiveGameData initialData)
        {
            return new GameManager(this);
        }

        public override int GetUID() => 100;

        public override string GetName() => "Normal Chess";

        public override string GetDescription()
        {
            return @"Normal Chess

Must have one player on both the black and white team

Traditional chess played on an 8x8 board";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };

        public override string[] TeamAliases() => new string[] { "White", "Black" };
    }

    /// <summary>
    /// Represents the state of the kings
    /// </summary>
    public enum KingsAlive
    {
        None = 0,
        White = 1,
        Black = 2,
        Both = 3,
    }

    /// <summary>
    /// Game Manager for Normal Chess
    /// </summary>
    public class GameManager : AbstractGameManager
    {
        /// <summary>
        /// Set to true when normal movement is cancelled e.g. promoting a pawn
        /// </summary>
        public bool CancelDefaultMove;

        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new Board(this);
        }

        public override List<Move> GetMoves(LiveGameData gameData, bool fastMode)
        {
            ((Board) Board).VirtualTeam = gameData.CurrentTeam;
            var possible_moves = Board.GetMoves(null);

            if (fastMode) return possible_moves;
            // Test if possible moves leave king in check
            var i = 0; 
            while (i < possible_moves.Count)
            {
                var temp_board = ((Board) Board).Clone(this) as Board; // Clone
                FalseOnMove(temp_board, possible_moves[i], gameData); // Make move
                temp_board.VirtualTeam = GUtil.SwitchTeam(gameData); // Change team

                // Check enemy moves for check
                var possible_enemy_moves = temp_board.GetMoves(null);
                var failed = possible_enemy_moves.Select(move => temp_board.GetPiece(move.To))
                    .Any(piece => piece is not null && piece.GetUID() == PieceUIDs.KING && piece.Team == gameData.CurrentTeam);

                // Remove illegal move
                if (failed)
                {
                    possible_moves.RemoveAt(i);
                }

                else i++;
            }

            return possible_moves;
        }

        /// <summary>
        /// Returns whether each king is alive
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        protected KingsAlive CheckForKings(AbstractBoard board)
        {
            var white_king = false;
            var black_king = false;
            for (var x = 0; x < board.PieceBoard.GetLength(0); x++)
            {
                for (var y = 0; y < board.PieceBoard.GetLength(1); y++)
                {
                    if (board.PieceBoard[x, y] is null) continue;
                    if (board.PieceBoard[x, y].GetUID() != PieceUIDs.KING) continue;
                    if (board.PieceBoard[x, y].Team == 0) white_king = true;
                    else black_king = true;
                }
            }

            if (white_king && black_king) return KingsAlive.Both;
            else if (white_king) return KingsAlive.White;
            else if (black_king) return KingsAlive.Black;
            else return KingsAlive.None;
        }

        /// <summary>
        /// Replacement for OnMove that can take any AbstractBoard state, not just the active one
        /// </summary>
        /// <param name="board"></param>
        /// <param name="move"></param>
        /// <param name="gameData"></param>
        /// <returns></returns>
        protected int FalseOnMove(AbstractBoard board, Move move, LiveGameData gameData)
        {
            CancelDefaultMove = false;

            board.OnMove(move);

            // Default movement
            if (!CancelDefaultMove)
            {
                board.PieceBoard[move.To.X, move.To.Y] = board.PieceBoard[move.From.X, move.From.Y];
                board.PieceBoard[move.To.X, move.To.Y].Position = move.To;
                board.PieceBoard[move.From.X, move.From.Y] = null;
            }

            ((Board) board).MoveCounter++;

            var kings_alive = CheckForKings(board);

            switch (kings_alive)
            {
                case KingsAlive.Black:
                    return GUtil.TurnEncodeTeam(1);
                case KingsAlive.White:
                case KingsAlive.None:
                    return GUtil.TurnEncodeTeam(0);
                default:
                    return GUtil.SwitchPlayerTeam(gameData);
            }
        }

        public override int OnMove(Move move, LiveGameData gameData) => FalseOnMove(Board, move, gameData);

        public override AbstractGameManager Clone()
        {
            var new_game_manager = new GameManager(GameManagerData);
            new_game_manager.Board = ((Board) Board).Clone(new_game_manager);
            return new_game_manager;
        }

        public override float GetScore(LiveGameData gameData)
        {
            var score = base.GetScore(gameData);

            // Add heatmap only if chess is on 8x8 board
            if (Board.PieceBoard.GetLength(0) == 8)
            {
                score += Heatmap.GetHeatmapScore(gameData, Board as Board);
            }

            return score;
        }
    }
}