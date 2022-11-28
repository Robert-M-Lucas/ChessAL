using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gamemodes;
using System.Net.NetworkInformation;
using TMPro;
using System;

namespace Game
{
    /// <summary>
    /// Stores theme data
    /// </summary>
    [System.Serializable]
    public class Theme
    {
        public string Name;

        [Space(7)]

        public Color WhiteColor;
        public Color BlackColor;

        [Space(7)]

        public Color WhiteHighlightColor;
        public Color BlackHighlightColor;

        [Space(7)]

        public Color WhiteSelectColor;
        public Color BlackSelectColor;

        [Space(7)]

        public Color WhiteBlockedColor;
        public Color BlackBlockedColor;

        [Space(7)]

        public Color WhiteMoveColor;
        public Color BlackMoveColor;
    }

    public class VisualManager : MonoBehaviour
    {
        public Theme[] Themes;
        private int currentTheme = 0;

        public RectTransform renderBox;

        public GameObject SquarePrefab;
        public GameObject PiecePrefab;
        public GameObject MoveOptionPrefab;

        public TMP_Text TurnText;
        public TMP_Text TeamWinText;
        public TMP_Text TimerText;
        public TMP_Text AIText;

        public AppearanceTable[] AppearanceTables;
        private Dictionary<int, Sprite> internalSpriteTable = new Dictionary<int, Sprite>();

        public ChessManager ChessManager;


        private const string PLAYER_PREFS_THEME_KEY = "Theme";

        private BoardRenderInfo boardRenderInfo;

        private Resolution resolution = new Resolution();

        private Dictionary<AbstractPiece, Image> piece_images = new Dictionary<AbstractPiece, Image>();

        private List<Move> possibleMoves = new List<Move>();
        private List<GameObject> moveIndicators = new List<GameObject>();
        private List<GameObject> pieces = new List<GameObject>();

        private Image[,] Squares;

        private V2? currentlyShowing = null;

        private List<V2> greyscaled = new List<V2>();

        // Run once
        private void Awake()
        {
            // Populate sprite table
            foreach (AppearanceTable appearance_table in AppearanceTables)
            {
                foreach (PieceSprite piece_sprite in appearance_table.Appearances)
                {
#if UNITY_EDITOR
                    if (internalSpriteTable.ContainsKey(piece_sprite.ID))
                    {
                        throw new Exception("Duplicate appearance ID");
                    }
#endif

                    internalSpriteTable[piece_sprite.ID] = piece_sprite.Sprite;
                }
            }

            if (PlayerPrefs.HasKey(PLAYER_PREFS_THEME_KEY))
            {
                currentTheme = PlayerPrefs.GetInt(PLAYER_PREFS_THEME_KEY);
                if (currentTheme >= Themes.Length) currentTheme = 0;
            }
        }

        // Run once
        void Start()
        {            
            boardRenderInfo = ChessManager.GameManager.Board.GetBoardRenderInfo();

            Squares = new Image[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];

            OnResolutionChange();
            RenderBoardBackground();
            UpdateAllPieces();
        }

        /// <summary>
        /// Shows that a team has won
        /// </summary>
        /// <param name="team"></param>
        public void OnTeamWin(int team)
        {
            string team_string;
            if (ChessManager.CurrentGameManager.TeamAliases().Length != 0)
            {
                team_string = ChessManager.CurrentGameManager.TeamAliases()[team];
            }
            else team_string = $"Team {team + 1}";

            TeamWinText.text = $"{team_string} won!";
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
                    if (ChessManager.GameManager.Board.PieceBoard[x, y] is not null) AddPiece(ChessManager.GameManager.Board.PieceBoard[x, y]);
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
        /// Returns whether the given position is white or black in the checkerboard
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsWhite(V2 position) => (position.X + position.Y) % 2 == 0;

        /// <summary>
        /// Renders the board's cells
        /// </summary>
        private void RenderBoardBackground()
        {
            for (int x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (int y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    GameObject new_square = Instantiate(SquarePrefab);

                    SizeGameObject(new_square, new V2(x, y));

                    RenderedCellData rendered_piece_data = new_square.GetComponent<RenderedCellData>();
                    rendered_piece_data.Position = new V2(x, y);

                    Image image = new_square.GetComponent<Image>();
                    Squares[x, y] = image;

                    ResetSquareColor(new V2(x, y));

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

            if (ChessManager.GameManager.Board.PieceBoard[clickPosition.X, clickPosition.Y] is null)
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
            SelectSquare((V2)currentlyShowing);

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

        /// <summary>
        /// Makes a square appear selected
        /// </summary>
        /// <param name="position"></param>
        public void SelectSquare(V2 position)
        {
            if (IsWhite(position)) Squares[position.X, position.Y].color = Themes[currentTheme].WhiteSelectColor;
            else Squares[position.X, position.Y].color = Themes[currentTheme].BlackSelectColor;
        }

        /// <summary>
        /// Marks a square as having had a piece move there last turn
        /// </summary>
        /// <param name="position"></param>
        public void ShowMove(V2 position)
        {
            if (IsWhite(position)) Squares[position.X, position.Y].color = Themes[currentTheme].WhiteMoveColor;
            else Squares[position.X, position.Y].color = Themes[currentTheme].BlackMoveColor;
        }

        /// <summary>
        /// Resets a square back to its default colour
        /// </summary>
        /// <param name="position"></param>
        public void ResetSquareColor(V2 position)
        {
            if (IsWhite(position))
            {
                if (boardRenderInfo.RemovedSquares.Contains(position))
                {
                    Squares[position.X, position.Y].color = Themes[currentTheme].WhiteBlockedColor;
                }
                else if (boardRenderInfo.HighlightedSquares.Contains(position))
                {
                    Squares[position.X, position.Y].color = Themes[currentTheme].WhiteHighlightColor;
                }
                else
                {
                    Squares[position.X, position.Y].color = Themes[currentTheme].WhiteColor;
                }
            }
            else
            {
                if (boardRenderInfo.RemovedSquares.Contains(position))
                {
                    Squares[position.X, position.Y].color = Themes[currentTheme].BlackBlockedColor;
                }
                else if (boardRenderInfo.HighlightedSquares.Contains(position))
                {
                    Squares[position.X, position.Y].color = Themes[currentTheme].BlackHighlightColor;
                }
                else
                {
                    Squares[position.X, position.Y].color = Themes[currentTheme].BlackColor;
                }
            }
        }

        /// <summary>
        /// Shows which piece moved on the last turn
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void OnMove(V2 from, V2 to)
        {
            foreach (V2 grey in greyscaled) ResetSquareColor(grey);
            greyscaled.Clear();

            ShowMove(from);
            ShowMove(to);
            greyscaled.Add(from);
            greyscaled.Add(to);
        }

        /// <summary>
        /// Updates the turn text on turn change
        /// </summary>
        /// <param name="team"></param>
        /// <param name="playerOnTeam"></param>
        /// <param name="you"></param>
        public void OnTurn(int team, int playerOnTeam, bool you, bool ai = false)
        {
            string team_string;
            if (ChessManager.CurrentGameManager.TeamAliases().Length != 0)
            {
                team_string = ChessManager.CurrentGameManager.TeamAliases()[team];
            }
            else team_string = $"T{team+1}";

            string str = $"Turn: {team_string}, P{playerOnTeam+1}";
            if (ai) str += " (AI)";
            else if (you) str += " (You)";
            TurnText.text = str;
        }

        /// <summary>
        /// Updates AI progress text
        /// </summary>
        /// <param name="waiting"></param>
        /// <param name="progress"></param>
        public void ShowAIInfo(bool waiting, float progress)
        {
            if (!waiting) AIText.gameObject.SetActive(false);
            else
            {
                AIText.gameObject.SetActive(true);
                AIText.text = $"AI: {Mathf.Round(progress)}%";
            }
        }

        private void UpdateTheme() 
        {
            for (int x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (int y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    ResetSquareColor(new V2(x, y));
                }
            }
        }

        void Update()
        {
            // Check for theme change input
            if (I.GetKeyDown(K.ChangeThemeKey))
            {
                Debug.Log("Change");
                currentTheme++;
                if (currentTheme >= Themes.Length) currentTheme = 0;
                UpdateTheme();
                PlayerPrefs.SetInt(PLAYER_PREFS_THEME_KEY, currentTheme);
            }

            // Check for resolution change
            if (resolution.height != Screen.height || resolution.width != Screen.width) OnResolutionChange();
            
            // Timer
            // TODO: Make this not run every frame
            long time = ChessManager.Timer.ElapsedMilliseconds + ChessManager.TimerOffset;
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