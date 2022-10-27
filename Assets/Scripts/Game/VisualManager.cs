using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gamemodes;
using System.Net.NetworkInformation;
using TMPro;

namespace Game
{
    public class VisualManager : MonoBehaviour
    {
        public RectTransform renderBox;

        public Image BlackSquare;
        public Image WhiteSquare;
        public GameObject BlockedSquare;
        public GameObject PiecePrefab;
        public GameObject MoveOptionPrefab;

        public TMP_Text TurnText;
        public TMP_Text TeamWinText;
        public TMP_Text TimerText;

        public float HighlightedSquareOpacity = 0.2f;
        public Color HighlightColor;

        public AppearanceTable[] AppearanceTables;
        private Dictionary<int, Sprite> internalSpriteTable = new Dictionary<int, Sprite>();

        private ChessManager chessManager;
        private BoardRenderInfo boardRenderInfo;

        private Resolution resolution = new Resolution();

        private Dictionary<AbstractPiece, Image> piece_images = new Dictionary<AbstractPiece, Image>();

        private List<Move> possibleMoves = new List<Move>();
        private List<GameObject> moveIndicators = new List<GameObject>();
        private List<GameObject> pieces = new List<GameObject>();

        private Image[,] Squares;

        private V2? currentlyShowing = null;

        private List<V2> greyscaled = new List<V2>();

        private void Awake()
        {
            foreach (AppearanceTable appearance_table in AppearanceTables)
            {
                foreach (PieceSprite piece_sprite in appearance_table.Appearances)
                {
                    internalSpriteTable[piece_sprite.ID] = piece_sprite.Sprite;
                }
            }
        }

        void Start()
        {
            chessManager = FindObjectOfType<ChessManager>();
            OnResolutionChange();
            // Render(new SampleBoard().GetBoardRenderInfo());
            boardRenderInfo = chessManager.GameManager.Board.GetBoardRenderInfo();
            Squares = new Image[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];
            RenderBoardBackground();
            UpdateAllPieces();
        }

        /// <summary>
        /// Shows that a team has won
        /// </summary>
        /// <param name="team"></param>
        public void OnTeamWin(int team)
        {
            TeamWinText.text = $"Team {team + 1} won!";
            TeamWinText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Renders all pieces in BoardManager
        /// </summary>
        public void UpdateAllPieces()
        {
            foreach (GameObject g in pieces) Destroy(g);
            pieces.Clear();

            for (int x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (int y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    if (chessManager.GameManager.Board.PieceBoard[x, y] is not null) AddPiece(chessManager.GameManager.Board.PieceBoard[x, y]);
                }
            }
        }

        /// <summary>
        /// Renders a specific piece
        /// </summary>
        /// <param name="piece"></param>
        private void AddPiece(AbstractPiece piece)
        {
            GameObject new_gameobject = Instantiate(PiecePrefab);
            pieces.Add(new_gameobject);
            Image image = new_gameobject.GetComponent<Image>();

            new_gameobject.SetActive(true);

            piece_images[piece] = image;

            UpdatePiece(piece);
        }

        /// <summary>
        /// Updates the visuals of a specific piece
        /// </summary>
        /// <param name="piece"></param>
        private void UpdatePiece(AbstractPiece piece)
        {
            Image image = piece_images[piece];
            image.sprite = internalSpriteTable[piece.AppearanceID];

            SizeGameObject(image.gameObject, piece.Position);
        }

        /// <summary>
        /// Resizes the board to keep the aspect ratio 1:1 on any screen size
        /// </summary>
        private void OnResolutionChange()
        {
            resolution.width = Screen.width;
            resolution.height = Screen.height;

            if (resolution.height > resolution.width)
            {
                renderBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, resolution.width);
                renderBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, resolution.width);
            }
            else
            {
                renderBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, resolution.height);
                renderBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, resolution.height);
            }
        }

        /// <summary>
        /// Renders the board's cells
        /// </summary>
        private void RenderBoardBackground()
        {
            for (int x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (int y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    GameObject square = null;

                    foreach (V2 blocked_square in boardRenderInfo.RemovedSquares)
                    {
                        if (x == blocked_square.X && y == blocked_square.Y)
                        {
                            square = BlockedSquare;
                            break;
                        }
                    }

                    if (square is null)
                    {
                        if ((x + y) % 2 == 0) square = WhiteSquare.gameObject;
                        else square = BlackSquare.gameObject;
                    }

                    GameObject new_square = Instantiate(square);
                    SizeGameObject(new_square, new V2(x, y));
                    RenderedCellData rendered_piece_data = new_square.GetComponent<RenderedCellData>();
                    rendered_piece_data.Position = new V2(x, y);

                    Image image = new_square.GetComponent<Image>();
                    Squares[x, y] = image;
                    if (boardRenderInfo.HighlightedSquares.Contains(new V2(x, y))) 
                    {
                        Color old_color = new_square.GetComponent<Image>().color;
                        image.color = new Color(old_color.r, old_color.g, old_color.b, HighlightedSquareOpacity);
                    }

                    new_square.SetActive(true);
                }
            }
        }

        // Gives a GameObject the correct size, scale and position
        private void SizeGameObject(GameObject gameObject, V2 position)
        {
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(renderBox);
            rect.anchorMin = new Vector2((float)position.X / boardRenderInfo.BoardSize, (float)position.Y / boardRenderInfo.BoardSize);
            rect.anchorMax = new Vector2((float)(position.X + 1) / boardRenderInfo.BoardSize, (float)(position.Y + 1) / boardRenderInfo.BoardSize);
            rect.localPosition = new Vector2((position.X + 0.5f) * (renderBox.rect.width / boardRenderInfo.BoardSize) - (renderBox.rect.width / 2),
                (position.Y + 0.5f) * (renderBox.rect.width / boardRenderInfo.BoardSize) - (renderBox.rect.width / 2));
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, renderBox.rect.width / boardRenderInfo.BoardSize);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, renderBox.rect.width / boardRenderInfo.BoardSize);
        }

        /// <summary>
        /// Updates the VisualManager's internal list of possible moves
        /// </summary>
        /// <param name="possibleMoves"></param>
        public void SetPossibleMoves(List<Move> possibleMoves)
        {
            this.possibleMoves = possibleMoves;
        }

        /// <summary>
        /// Hides the currently shown move indicators
        /// </summary>
        public void HideMoves()
        {
            if (currentlyShowing is not null) ResetSquareColor((V2)currentlyShowing);

            while (moveIndicators.Count > 0)
            {
                Destroy(moveIndicators[0]);
                moveIndicators.RemoveAt(0);
            }
        }

        /// <summary>
        /// Handles a click somewhere on the board
        /// </summary>
        /// <param name="clickPosition"></param>
        /// <returns></returns>
        public bool ToggleShowMoves(V2 clickPosition)
        {
            HideMoves();

            if (chessManager.GameManager.Board.PieceBoard[clickPosition.X, clickPosition.Y] is null)
            {
                currentlyShowing = null;
                return false;
            }

            if (currentlyShowing is not null && (V2)currentlyShowing == clickPosition)
            {
                currentlyShowing = null;
                return false;
            }

            currentlyShowing = clickPosition;
            HighlightSquare((V2)currentlyShowing);

            foreach (Move move in possibleMoves)
            {
                if (move.From == clickPosition)
                {
                    GameObject new_indicator = Instantiate(MoveOptionPrefab);
                    SizeGameObject(new_indicator, move.To);
                    new_indicator.SetActive(true);
                    moveIndicators.Add(new_indicator);
                }
            }

            return true;
        }

        public void HighlightSquare(V2 position)
        {
            Squares[position.X, position.Y].color += HighlightColor;
        }

        public void GreyscaleSquare(V2 position)
        { 
            Color base_color = Squares[position.X, position.Y].color;
            float color = ((base_color.r + base_color.g + base_color.b) / 3) * 0.75f;
            Squares[position.X, position.Y].color = new Color(color, color, color);
        }

        public void ResetSquareColor(V2 position)
        {
            if ((position.X + position.Y) % 2 == 0) Squares[position.X, position.Y].color = WhiteSquare.color;
            else Squares[position.X, position.Y].color = BlackSquare.color;

            if (boardRenderInfo.HighlightedSquares.Contains(position))
            {
                Color old_color = Squares[position.X, position.Y].color;
                Squares[position.X, position.Y].color = new Color(old_color.r, old_color.g, old_color.b, HighlightedSquareOpacity);
            }
        }

        public void OnMove(V2 from, V2 to)
        {
            foreach (V2 grey in greyscaled) ResetSquareColor(grey);
            greyscaled.Clear();

            GreyscaleSquare(from);
            GreyscaleSquare(to);
            greyscaled.Add(from);
            greyscaled.Add(to);
        }

        public void OnTurn(int team, int playerOnTeam, bool you)
        {
            string str = $"Turn: T{team}, P{playerOnTeam}";
            if (you) str += " (You)";
            TurnText.text = str;
        }

        void Update()
        {
            // Check for resolution change
            if (resolution.height != Screen.height || resolution.width != Screen.width) OnResolutionChange();
            
            // TODO: Make this not run every frame
            long time = chessManager.Timer.ElapsedMilliseconds + chessManager.TimerOffset;
            long hours = time / (60 * 60 * 1000);
            time -= hours * (60 * 60 * 1000);
            long minutes = time / (60 * 1000);
            time -= minutes * (60 * 1000);
            long seconds = time / (1000);

            string time_string = $"{seconds}";
            while (time_string.Length < 2) time_string = "0" + time_string;
            time_string = ":" + time_string;
            time_string = minutes + time_string;
            while (time_string.Length < 5) time_string = "0" + time_string;
            time_string = ":" + time_string;
            time_string = hours + time_string;
            while (time_string.Length < 8) time_string = "0" + time_string;
            TimerText.text = time_string;
        }
    }
}