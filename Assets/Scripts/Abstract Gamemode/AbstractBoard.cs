using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes
{
    /// <summary>
    /// Handles the board's configuration and stores pieces
    /// </summary>
    public abstract class AbstractBoard
    {
        public AbstractPiece[,] PieceBoard;

        protected AbstractGameManager gameManager;

        public AbstractBoard(AbstractGameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public virtual List<Move> GetMoves()
        {
            IEnumerable<Move> moves = new List<Move>();
            for (int x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < PieceBoard.GetLength(1); y++)
                {
                    if (PieceBoard[x, y] is not null && PieceBoard[x, y].Team == gameManager.chessManager.GetLocalPlayerTeam()) moves = moves.Concat(PieceBoard[x, y].GetMoves());
                }
            }
            
            return GamemodeUtil.RemoveBlocked(moves.ToList(), this);
        }

        public abstract BoardRenderInfo GetBoardRenderInfo();

        public virtual void OnMove(V2 from, V2 to)
        {
            for (int x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < PieceBoard.GetLength(1); y++)
                {
                    if (PieceBoard[x, y] is not null) PieceBoard[x, y].OnMove(from, to);
                }
            }
        }
    }
}