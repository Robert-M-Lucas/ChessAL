using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate()
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

    public class GameManager : AbstractGameManager
    {
        
        public bool CancelDefaultMove;

        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new Board(this);
        }

        public override List<Move> GetMoves(LiveGameData gameData)
        {
            (Board as Board).VirtualTeam = gameData.LocalPlayerTeam;
            List<Move> possible_moves = Board.GetMoves(null);
            
            int i = 0;
            while (i < possible_moves.Count)
            {
                Board temp_board = (Board as Board).Clone(this) as Board;
                FalseOnMove(temp_board, possible_moves[i], gameData);
                temp_board.VirtualTeam = GUtil.SwitchTeam(gameData);

                bool failed = false;
                List<Move> possible_enemy_moves = temp_board.GetMoves(null);
                foreach (Move move in possible_enemy_moves)
                {
                    AbstractPiece piece = temp_board.GetPiece(move.To);
                    if (piece is not null && piece.GetUID() == PieceUIDs.KING && piece.Team == gameData.LocalPlayerTeam)
                    {
                        failed = true;
                        break;
                    }
                }

                if (failed)
                {
                    possible_moves.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            return possible_moves;
        }

        protected int FalseOnMove(AbstractBoard board, Move move, LiveGameData gameData)
        {
            CancelDefaultMove = false;

            board.OnMove(move);

            if (!CancelDefaultMove)
            {
                board.PieceBoard[move.To.X, move.To.Y] = board.PieceBoard[move.From.X, move.From.Y];
                board.PieceBoard[move.To.X, move.To.Y].Position = move.To;
                board.PieceBoard[move.From.X, move.From.Y] = null;
            }

            (board as Board).MoveCounter++;

            bool white_king = false;
            bool black_king = false;
            for (int x = 0; x < board.PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < board.PieceBoard.GetLength(1); y++)
                {
                    if (board.PieceBoard[x, y] is not null)
                    {
                        if (board.PieceBoard[x, y].GetUID() == PieceUIDs.KING)
                        {
                            if (board.PieceBoard[x, y].Team == 0) white_king = true;
                            else black_king = true;
                        }
                    }
                }
            }

            if (!white_king) return GUtil.TurnEncodeTeam(1);
            if (!black_king) return GUtil.TurnEncodeTeam(0);

            return GUtil.SwitchPlayerTeam(gameData);
        }

        public override int OnMove(Move move, LiveGameData gameData)
        {
            return FalseOnMove(Board, move, gameData);
        }

        public override AbstractGameManager Clone()
        {
            GameManager gameManager = new GameManager(GameManagerData);
            gameManager.Board = Board.Clone(this);
            return gameManager;
        }
    }
}