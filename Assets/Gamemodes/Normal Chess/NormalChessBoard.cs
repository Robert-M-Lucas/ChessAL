using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Gamemodes.NormalChess
{
    /// <summary>
    /// Sample Board
    /// </summary>
    public class Board : AbstractBoard
    {
        public int MoveCounter;
        public int VirtualTeam;

        public Board(AbstractGameManager gameManager, bool initialise = true) : base(gameManager)
        {
            if (initialise) Initialise();
        }

        protected void Initialise()
        {
            PieceBoard = new AbstractPiece[8, 8];
            PieceBoard[0, 1] = new PawnPiece(new V2(0, 1), 0, this);
            PieceBoard[1, 1] = new PawnPiece(new V2(1, 1), 0, this);
            PieceBoard[2, 1] = new PawnPiece(new V2(2, 1), 0, this);
            PieceBoard[3, 1] = new PawnPiece(new V2(3, 1), 0, this);
            PieceBoard[4, 1] = new PawnPiece(new V2(4, 1), 0, this);
            PieceBoard[5, 1] = new PawnPiece(new V2(5, 1), 0, this);
            PieceBoard[6, 1] = new PawnPiece(new V2(6, 1), 0, this);
            PieceBoard[7, 1] = new PawnPiece(new V2(7, 1), 0, this);

            PieceBoard[0, 6] = new PawnPiece(new V2(0, 6), 1, this);
            PieceBoard[1, 6] = new PawnPiece(new V2(1, 6), 1, this);
            PieceBoard[2, 6] = new PawnPiece(new V2(2, 6), 1, this);
            PieceBoard[3, 6] = new PawnPiece(new V2(3, 6), 1, this);
            PieceBoard[4, 6] = new PawnPiece(new V2(4, 6), 1, this);
            PieceBoard[5, 6] = new PawnPiece(new V2(5, 6), 1, this);
            PieceBoard[6, 6] = new PawnPiece(new V2(6, 6), 1, this);
            PieceBoard[7, 6] = new PawnPiece(new V2(7, 6), 1, this);

            PieceBoard[4, 0] = new KingPiece(new V2(4, 0), 0, this);
            PieceBoard[4, 7] = new KingPiece(new V2(4, 7), 1, this);

            PieceBoard[1, 0] = new KnightPiece(new V2(1, 0), 0, this);
            PieceBoard[6, 0] = new KnightPiece(new V2(6, 0), 0, this);
            PieceBoard[1, 7] = new KnightPiece(new V2(1, 7), 1, this);
            PieceBoard[6, 7] = new KnightPiece(new V2(6, 7), 1, this);

            PieceBoard[2, 0] = new BishopPiece(new V2(2, 0), 0, this);
            PieceBoard[5, 0] = new BishopPiece(new V2(5, 0), 0, this);
            PieceBoard[2, 7] = new BishopPiece(new V2(2, 7), 1, this);
            PieceBoard[5, 7] = new BishopPiece(new V2(5, 7), 1, this);

            PieceBoard[0, 0] = new RookPiece(new V2(0, 0), 0, this);
            PieceBoard[7, 0] = new RookPiece(new V2(7, 0), 0, this);
            PieceBoard[0, 7] = new RookPiece(new V2(0, 7), 1, this);
            PieceBoard[7, 7] = new RookPiece(new V2(7, 7), 1, this);

            PieceBoard[3, 0] = new QueenPiece(new V2(3, 0), 0, this);
            PieceBoard[3, 7] = new QueenPiece(new V2(3, 7), 1, this);
        }

        public override SerialisationData GetData()
        {
            SerialisationData data = base.GetData();
            data.BoardData = BitConverter.GetBytes(MoveCounter);
            return data;
        }

        public override void LoadData(SerialisationData data)
        {
            MoveCounter = BitConverter.ToInt32(data.BoardData);
            base.LoadData(data);
        }

        public override void OnMove(Move move)
        {
            PieceBoard[move.From.X, move.From.Y].OnMove(move);
        }

        public override List<Move> GetMoves(LiveGameData gameData)
        {
            IEnumerable<Move> moves = new List<Move>();
            for (int x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < PieceBoard.GetLength(1); y++)
                {
                    if (PieceBoard[x, y] is not null && PieceBoard[x, y].Team == VirtualTeam) moves = moves.Concat(PieceBoard[x, y].GetMoves());
                }
            }

            return GUtil.RemoveBlocked(moves.ToList(), this);
        }

        public override AbstractBoard Clone(AbstractGameManager newGameManager)
        {
            Board new_board = new Board(newGameManager, false);
            
            new_board.MoveCounter = MoveCounter;
            new_board.VirtualTeam = VirtualTeam;
            new_board.PieceBoard = new AbstractPiece[PieceBoard.GetLength(0), PieceBoard.GetLength(1)];

            for (int x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < PieceBoard.GetLength(1); y++)
                {
                    if (PieceBoard[x, y] is not null)
                    {
                        new_board.PieceBoard[x, y] = PieceBoard[x, y].Clone(new_board);
                    }
                }
            }
            return new_board;
        }

        public override BoardRenderInfo GetBoardRenderInfo()
        {
            return new BoardRenderInfo(8, new List<V2>());
        }
    }
}