using System.Collections.Generic;
using System.Linq;
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
            var winning_squares = new List<V2>() { new V2(0, 0), new V2(10, 0), new V2(0, 10), new V2(10, 10) };

            // If king is in winning square
            if (winning_squares.Any(square => Board.GetPiece(square) is not null && Board.GetPiece(square).GetUID() == PieceUIDs.KING))
            {
                return GUtil.TurnEncodeTeam(1);
            }

            var found_king = false;
            for (var x = 0; x < 11; x++)
            {
                for (var y = 0; y < 11; y++)
                {
                    if (Board.PieceBoard[x, y] is null || Board.PieceBoard[x, y].GetUID() != PieceUIDs.KING) continue;
                    found_king = true;
                    break;
                }
                if (found_king) break;
            }

            return !found_king ? GUtil.TurnEncodeTeam(0) : // If king not found
                0;
        }

        /// <summary>
        /// Checks for a piece being taken
        /// </summary>
        /// <param name="to"></param>
        /// <param name="gameData"></param>
        private void CheckForTake(V2 to, LiveGameData gameData)
        {
            var turn = gameData.GetPlayerList()[gameData.CurrentPlayer].Team;

            for (var x = 0; x < 11; x++)
            {
                for (var y = 0; y < 11; y++)
                {
                    if (Board.PieceBoard[x, y] is null || Board.PieceBoard[x, y].Team == turn) continue;
                    var neighbours = new List<V2>();

                    var active = false;
                    var king = Board.PieceBoard[x, y].GetUID() == PieceUIDs.KING;

                    var current_pos = new V2(x, y);

                    var around = new List<V2>() { new V2(1, 0), new V2(-1, 0), new V2(0, 1), new V2(0, -1) };

                    var winning_squares = new List<V2>() { new V2(0, 0), new V2(10, 0), new V2(0, 10), new V2(10, 10) };

                    // Find status of squares around piece
                    foreach (var pos in around)
                    {
                        if (king && !GUtil.IsOnBoard(current_pos + pos, Board))
                        {
                            neighbours.Add(pos);
                            continue;
                        }

                        if (!GUtil.IsOnBoard(current_pos + pos, Board)) continue;

                        if (winning_squares.Contains(current_pos + pos))
                        {
                            neighbours.Add(pos);
                            continue;
                        }

                        if (Board.GetPiece(current_pos + pos) is not null && 
                            Board.GetPiece(current_pos + pos).Team == turn && 
                            Board.GetPiece(current_pos+pos).GetUID() == PieceUIDs.PIECE)
                        {
                            if (to == current_pos + pos) active = true;
                            neighbours.Add(pos);
                            continue;
                        }

                        if (king || (current_pos + pos) != VikingChess.Board.CENTRE) continue;
                        neighbours.Add(pos);
                    }

                    // Piece hasn't moved this turn
                    if (!active) continue;

                    switch (king)
                    {
                        // King is surrounded
                        case true when neighbours.Count == 4:
                            Board.PieceBoard[x, y] = null;
                            continue;
                        // Piece is surrounded on 2 sides
                        case false when neighbours.Count >= 2:
                        {
                            if (neighbours.Contains(new V2(0, 1)) && neighbours.Contains(new V2(0, -1)) && (to == current_pos + new V2(0, 1) || to == current_pos + new V2(0, -1)))  
                            {
                                Board.PieceBoard[x, y] = null;
                            }
                            else if (neighbours.Contains(new V2(1, 0)) && neighbours.Contains(new V2(-1, 0)) && (to == current_pos + new V2(1, 0) || to == current_pos + new V2(-1, 0)))
                            {
                                Board.PieceBoard[x, y] = null;
                            }

                            break;
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

            var check_for_win = CheckForWin();
            return check_for_win < 0 ? check_for_win : GUtil.SwitchPlayerTeam(gameData);
        }

        public override AbstractGameManager Clone()
        {
            var new_game_manager = new VikingChess.GameManager(GameManagerData);
            new_game_manager.Board = ((Board) Board).Clone(new_game_manager);
            return new_game_manager;
        }
    }
}