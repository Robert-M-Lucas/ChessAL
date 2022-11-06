using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.VikingChess
{
    public class GameManagerData : AbstractGameManagerData
    {
        public override AbstractGameManager Instantiate(ChessManager chessManager)
        {
            return new GameManager(this, chessManager);
        }

        public override int GetUID() => 800;

        public override string GetName() => "Viking Chess";

        public override string GetDescription()
        {
            return @"Viking Chess

Must have 2 teams of 1

Get your king to a corner of the board. Surround a piece on two sides to take it or surround the king on all sides to win the game.";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : AbstractGameManager
    {
        public GameManager(AbstractGameManagerData d, ChessManager chessManager) : base(d, chessManager)
        {
            Board = new Board(this);
        }

        private int CheckForWin()
        {
            List<V2> winning_squares = new List<V2>() { new V2(0, 0), new V2(10, 0), new V2(0, 10), new V2(10, 10) };

            foreach (V2 square in winning_squares)
            {
                if (Board.GetPiece(square) is not null && Board.GetPiece(square).GetUID() == PieceUIDs.King) return GUtil.TurnEncodeTeam(0);
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

            if (!found_king) return GUtil.TurnEncodeTeam(1);

            return 0;
        }

        private void CheckForTake(V2 to)
        {
            int turn = chessManager.GetPlayerList()[chessManager.CurrentPlayer].Team;

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

                            if (Board.GetPiece(current_pos + pos) is not null && Board.GetPiece(current_pos + pos).Team == turn && Board.GetPiece(current_pos+pos).GetUID() == PieceUIDs.Piece)
                            {
                                if (to == current_pos + pos) active = true;
                                neigbours.Add(pos);
                                continue;
                            }
                        }

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

        public override int OnMove(V2 from, V2 to)
        {
            Board.PieceBoard[to.X, to.Y] = Board.PieceBoard[from.X, from.Y];
            Board.PieceBoard[to.X, to.Y].Position = to;
            Board.PieceBoard[from.X, from.Y] = null;

            CheckForTake(to);

            int check_for_win = CheckForWin();
            if (check_for_win < 0) return check_for_win;

            return GUtil.SwitchPlayerTeam(chessManager);
        }
    }
}