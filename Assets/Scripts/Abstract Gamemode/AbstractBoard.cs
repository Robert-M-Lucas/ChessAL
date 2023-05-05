using System;
using System.Collections.Generic;
using System.Linq;
using Game;

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
			GameManager = gameManager;
		}

		/// <summary>
		/// Returns serialised data
		/// </summary>
		/// <returns></returns>
		public virtual SerialisationData GetData()
		{
			var serialisation_data = new SerialisationData();

			// Add data for every piece on board
            for (var x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (var y = 0; y < PieceBoard.GetLength(1); y++)
                {
	                if (PieceBoard[x, y] is null) continue;
	                var data = PieceBoard[x, y].GetData();
	                if (data is not null) serialisation_data.PieceData.Add(data);
                }
            }

            return serialisation_data;
        }

		/// <summary>
		/// Loads serialised data
		/// </summary>
		/// <param name="data"></param>
		public virtual void LoadData(SerialisationData data)
		{
			PieceBoard = new AbstractPiece[PieceBoard.GetLength(0), PieceBoard.GetLength(1)];

			// Get map of UIDs to pieces
			var all_pieces = Util.GetAllPieces();
			var mapped_pieces = new Dictionary<int, Type>();
			foreach (var piece in all_pieces) mapped_pieces[piece.GetUID()] = piece.GetType();

			// Create instance of every piece in save data and place it on the board
			foreach (var piece_data in data.PieceData)
			{
				var new_piece = (AbstractPiece)Activator.CreateInstance(mapped_pieces[piece_data.UID], new object[] { piece_data.Position, piece_data.Team, this });
				new_piece.LoadData(piece_data); // Load piece data
				PieceBoard[piece_data.Position.X, piece_data.Position.Y] = new_piece;
			}
		}

		/// <summary>
		/// Returns a list of possible moves
		/// </summary>
		/// <param name="gameData"></param>
		/// <returns></returns>
		public virtual List<Move> GetMoves(LiveGameData gameData)
		{
			IEnumerable<Move> moves = new List<Move>();
			for (var x = 0; x < PieceBoard.GetLength(0); x++)
			{
				for (var y = 0; y < PieceBoard.GetLength(1); y++)
				{
					if (PieceBoard[x, y] is not null && PieceBoard[x, y].Team == gameData.CurrentTeam) moves = moves.Concat(PieceBoard[x, y].GetMoves());
				}
			}
			
			return GUtil.RemoveBlocked(moves.ToList(), this);
		}

		/// <summary>
		/// Returns a piece at the given position
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public AbstractPiece GetPiece(V2 position)
		{
			return PieceBoard[position.X, position.Y];
		}

		/// <summary>
		/// Returns the render information for the board
		/// </summary>
		/// <returns></returns>
		public abstract BoardRenderInfo GetBoardRenderInfo();

		/// <summary>
		/// Applies a move
		/// </summary>
		/// <param name="move"></param>
		public virtual void OnMove(Move move)
		{
			for (var x = 0; x < PieceBoard.GetLength(0); x++)
			{
				for (var y = 0; y < PieceBoard.GetLength(1); y++)
				{
					if (PieceBoard[x, y] is not null) PieceBoard[x, y].OnMove(move, move.From == new V2(x, y));
				}
			}
		}

		/// <summary>
		/// Returns a score for the board
		/// </summary>
		/// <param name="gameData"></param>
		/// <returns></returns>
		public virtual float GetScore(LiveGameData gameData)
		{
			float total = 0;

            for (var x = 0; x < PieceBoard.GetLength(0); x++)
            {
                for (var y = 0; y < PieceBoard.GetLength(1); y++)
                {
	                if (PieceBoard[x, y] is null) continue;
	                if (PieceBoard[x, y].Team == gameData.LocalPlayerTeam)
	                {
		                total += PieceBoard[x, y].GetValue();
	                }
	                else
	                {
		                total -= PieceBoard[x, y].GetValue();
	                }
                }
            }

			return total;
        }

		/// <summary>
		/// Returns a clone of the board
		/// </summary>
		/// <param name="newGameManager"></param>
		/// <returns></returns>
		public abstract AbstractBoard Clone(AbstractGameManager newGameManager);
	}
}