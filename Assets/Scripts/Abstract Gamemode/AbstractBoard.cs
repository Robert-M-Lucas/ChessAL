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

		public AbstractGameManager GameManager;

		public AbstractBoard(AbstractGameManager gameManager)
		{
			this.GameManager = gameManager;
		}

		public virtual List<Move> GetMoves()
		{
			IEnumerable<Move> moves = new List<Move>();
			for (int x = 0; x < PieceBoard.GetLength(0); x++)
			{
				for (int y = 0; y < PieceBoard.GetLength(1); y++)
				{
					if (PieceBoard[x, y] is not null && PieceBoard[x, y].Team == GameManager.chessManager.GetLocalPlayerTeam()) moves = moves.Concat(PieceBoard[x, y].GetMoves());
				}
			}
			
			return GamemodeUtil.RemoveBlocked(moves.ToList(), this);
		}

		public AbstractPiece GetPiece(V2 position)
		{
			return PieceBoard[position.X, position.Y];
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