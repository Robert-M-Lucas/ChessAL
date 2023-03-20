using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Gamemodes.VikingChess
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(LiveGameData initialData)
        {
            return new GameManager(this);
        }
        public override int GetUID() => 800;
        public override string GetName() => "Viking Chess";
        public override string GetDescription()
        {
            return @"Viking Chess

Must have one player on both the black and white team

Get your king to a corner of the board. Surround a piece on two sides to take it or surround the king on all sides to win the game.";
        }
        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
        public override string[] TeamAliases() => new string[] { "Black", "White" };
    }

    public class GameManager : AbstractGameManager
    {
        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new Board(this);
        }

        private int CheckForWin()
        {
            List<V2> winning_squares = new List<V2>() { new V2(0, 0), new V2(10, 0), new V2(0, 10), new V2(10, 10) };

            // If king is in winning square
            foreach (V2 square in winning_squares)
            {
                if (Board.GetPiece(square) is not null && Board.GetPiece(square).GetUID() == PieceUIDs.King) return GUtil.TurnEncodeTeam(1);
            }

            bool found_king = false;
            for (int x = 0; x < 11; x++)
            {
                for (int y = 0; y < 11; y++)
                {
                    if (Board.PieceBoard[x, y] is not null && Board.PieceBoard[x, y].GetUID() == PieceUIDs.King)
                    {
                        found_king = true;
                        break;
                    }
                }
                if (found_king) break;
            }

            if (!found_king) return GUtil.TurnEncodeTeam(0); // If king not found

            return 0;
        }

        /// <summary>
        /// Checks for a piece being taken
        /// </summary>
        /// <param name="to"></param>
        /// <param name="gameData"></param>
        private void CheckForTake(V2 to, LiveGameData gameData)
        {
            int turn = gameData.GetPlayerList()[gameData.CurrentPlayer].Team;

            for (int x = 0; x < 11; x++)
            {
                for (int y = 0; y < 11; y++)
                {
                    if (Board.PieceBoard[x, y] is not null && Board.PieceBoard[x, y].Team != turn)
                    {
                        List<V2> neigbours = new List<V2>();

                        bool active = false;
                        bool king = Board.PieceBoard[x, y].GetUID() == PieceUIDs.King;

                        V2 current_pos = new V2(x, y);

                        List<V2> around = new List<V2>() { new V2(1, 0), new V2(-1, 0), new V2(0, 1), new V2(0, -1) };

                        List<V2> winning_squares = new List<V2>() { new V2(0, 0), new V2(10, 0), new V2(0, 10), new V2(10, 10) };

                        // Find status of squares around piece
                        foreach (V2 pos in around)
                        {
                            if (king && !GUtil.IsOnBoard(current_pos + pos, Board))
                            {
                                neigbours.Add(pos);
                                continue;
                            }
                            else if (!GUtil.IsOnBoard(current_pos + pos, Board)) continue;

                            if (winning_squares.Contains(current_pos + pos))
                            {
                                neigbours.Add(pos);
                                continue;
                            }

                            if (Board.GetPiece(current_pos + pos) is not null && 
                                Board.GetPiece(current_pos + pos).Team == turn && 
                                Board.GetPiece(current_pos+pos).GetUID() == PieceUIDs.Piece)
                            {
                                if (to == current_pos + pos) active = true;
                                neigbours.Add(pos);
                                continue;
                            }

                            if (!king && (current_pos + pos) == VikingChess.Board.CENTRE) 
                            {
                                neigbours.Add(pos);
                                continue;
                            }
                        }

                        // Piece hasn't moved this turn
                        if (!active) continue;

                        // King is surrounded
                        if (king && neigbours.Count == 4)
                        {
                            Board.PieceBoard[x, y] = null;
                            continue;
                        }

                        // Piece is surrounded on 2 sides
                        if (!king && neigbours.Count >= 2)
                        {
                            if (neigbours.Contains(new V2(0, 1)) && neigbours.Contains(new V2(0, -1)) && (to == current_pos + new V2(0, 1) || to == current_pos + new V2(0, -1)))  
                            {
                                Board.PieceBoard[x, y] = null;
                                continue;
                            }
                            else if (neigbours.Contains(new V2(1, 0)) && neigbours.Contains(new V2(-1, 0)) && (to == current_pos + new V2(1, 0) || to == current_pos + new V2(-1, 0)))
                            {
                                Board.PieceBoard[x, y] = null;
                                continue;
                            }
                        }
                    }
                }
                
            }
        }

        public override int OnMove(Move move, LiveGameData gameData)
        {
            Board.PieceBoard[move.To.X, move.To.Y] = Board.PieceBoard[move.From.X, move.From.Y];
            Board.PieceBoard[move.To.X, move.To.Y].Position = move.To;
            Board.PieceBoard[move.From.X, move.From.Y] = null;

            CheckForTake(move.To, gameData);

            int check_for_win = CheckForWin();
            if (check_for_win < 0) return check_for_win;

            return GUtil.SwitchPlayerTeam(gameData);
        }

        public override AbstractGameManager Clone()
        {
            GameManager new_game_manager = new VikingChess.GameManager(GameManagerData);
            new_game_manager.Board = (Board as VikingChess.Board).Clone(new_game_manager);
            return new_game_manager;
        }
    }
}