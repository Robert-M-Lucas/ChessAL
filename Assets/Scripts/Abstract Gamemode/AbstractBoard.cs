using System;
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

		public virtual SerialisationData GetData()
		{
			SerialisationData serialisationData = new SerialisationData();

            IEnumerable<Move> moves = new List<Move>();
            for (int x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (int y = 0; y < PieceBoard.GetLength(1); y++)
                {
                    if (PieceBoard[x, y] is not null)
					{
						PieceSerialisationData data = PieceBoard[x, y].GetData();
                        if (data is not null)
						{
							serialisationData.PieceData.Add(data);
						}
					}
                }
            }

            return serialisationData;
        }

		public virtual void LoadData(SerialisationData data)
		{
			PieceBoard = new AbstractPiece[PieceBoard.GetLength(0), PieceBoard.GetLength(1)];
			List<AbstractPiece> all_pieces = Util.GetAllPieces();
			Dictionary<int, Type> mapped_pieces = new Dictionary<int, Type>();
			foreach (AbstractPiece piece in all_pieces) mapped_pieces[piece.GetUID()] = piece.GetType();

			foreach (PieceSerialisationData piece_data in data.PieceData)
			{
				AbstractPiece new_piece = (AbstractPiece)Activator.CreateInstance(mapped_pieces[piece_data.UID], new object[] { piece_data.Position, piece_data.Team, this });
				new_piece.LoadData(piece_data);
				PieceBoard[piece_data.Position.X, piece_data.Position.Y] = new_piece;
			}
		}

		public virtual List<Move> GetMoves(LiveGameData gameData)
		{
			IEnumerable<Move> moves = new List<Move>();
			for (int x = 0; x < PieceBoard.GetLength(0); x++)
			{
				for (int y = 0; y < PieceBoard.GetLength(1); y++)
				{
					if (PieceBoard[x, y] is not null && PieceBoard[x, y].Team == gameData.LocalPlayerTeam) moves = moves.Concat(PieceBoard[x, y].GetMoves());
				}
			}
			
			return GUtil.RemoveBlocked(moves.ToList(), this);
		}

		public AbstractPiece GetPiece(V2 position)
		{
			return PieceBoard[position.X, position.Y];
		}

		public abstract BoardRenderInfo GetBoardRenderInfo();

		public virtual void OnMove(Move move)
		{
			for (int x = 0; x < PieceBoard.GetLength(0); x++)
			{
				for (int y = 0; y < PieceBoard.GetLength(1); y++)
				{
					if (PieceBoard[x, y] is not null) PieceBoard[x, y].OnMove(move);
				}
			}
		}
	}
}