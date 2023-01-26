using Gamemodes.NormalChess;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Gamemodes.LastPieceStanding
{
    public class GameManagerData : NormalChess.GameManagerData
    {
        public override AbstractGameManager Instantiate()
        {
            return new GameManager(this);
        }

        public override int GetUID() => 600;

        public override string GetName() => "Last Piece Standing";

        public override string GetDescription()
        {
            return @"Last Piece Standing

Must have one player on both the black and white team

Chess played until one team has no pieces remaining";
        }

        public override TeamSize[] GetTeamSizes() => new TeamSize[] { new TeamSize(1, 1), new TeamSize(1, 1) };
    }

    public class GameManager : NormalChess.GameManager
    {
        public GameManager(AbstractGameManagerData d) : base(d)
        {
            Board = new NormalChess.Board(this);
        }

        // Override chess get moves to ignore check and king loss
        public override List<Move> GetMoves(LiveGameData gameData, bool fastMode)
        {
            (Board as Board).VirtualTeam = gameData.CurrentTeam;
            return Board.GetMoves(gameData);
        }

        public override int OnMove(Move move, LiveGameData gameData)
        {
            CancelDefaultMove = false;

            Board.OnMove(move);

            if (!CancelDefaultMove)
            {
                Board.PieceBoard[move.To.X, move.To.Y] = Board.GetPiece(move.From);
                Board.PieceBoard[move.To.X, move.To.Y].Position = move.To;
                Board.PieceBoard[move.From.X, move.From.Y] = null;
            }

            (Board as Board).MoveCounter++;

            return GUtil.SwitchTeam(gameData);
        }
    }
}

