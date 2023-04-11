using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gamemodes;
using TMPro;
using System;
using Game.ThreeD;
using Game.UI;
using UnityEngine.Serialization;

namespace Game
{
    /// <summary>
    /// Stores theme data
    /// </summary>
    [Serializable]
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
        [Space(7)]
        public Color White3DHighlightColor;
        public Color Black3DHighlightColor;
        [Space(7)]
        public Color White3DTakeHighlightColor;
        public Color Black3DTakeHighlightColor;
    }

    [RequireComponent(typeof(GameMenuManager))]
    public class VisualManager : MonoBehaviour
    {
        public VisualManagerThreeD VisualManager3D;

        [FormerlySerializedAs("pieceTravelTime")] public float PieceTravelTime;

        [SerializeField] private Theme[] themes;
        [SerializeField] private int currentTheme = 0;

        [SerializeField] private RectTransform renderBox;

        [FormerlySerializedAs("Hide3DImage")] [SerializeField] private Image hide3DImage;

        [FormerlySerializedAs("SquarePrefab")] [SerializeField] private GameObject squarePrefab;
        [FormerlySerializedAs("PiecePrefab")] [SerializeField] private GameObject piecePrefab;
        [FormerlySerializedAs("MoveOptionPrefab")] [SerializeField] private GameObject moveOptionPrefab;

        [FormerlySerializedAs("TurnText")] [SerializeField] private TMP_Text turnText;
        [FormerlySerializedAs("TeamWinText")] [SerializeField] private TMP_Text teamWinText;
        [FormerlySerializedAs("TimerText")] [SerializeField] private TMP_Text timerText;
        [FormerlySerializedAs("AIText")] [SerializeField] private TMP_Text aiText;

        [FormerlySerializedAs("AppearanceTables")] [SerializeField] private AppearanceTable[] appearanceTables;
        public Dictionary<int, PieceAppearance> InternalAppearanceMap = new Dictionary<int, PieceAppearance>();

        public ChessManager ChessManager;
        public GameMenuManager GameMenuManager;

        private const string PLAYER_PREFS_THEME_KEY = "Theme";

        private BoardRenderInfo boardRenderInfo;

        private Resolution resolution = new Resolution();

        [FormerlySerializedAs("possibleMoves")] public List<Move> PossibleMoves = new List<Move>();
        private List<GameObject> moveIndicators = new List<GameObject>();

        private Dictionary<V2, GameObject> pieces2D = new Dictionary<V2, GameObject>();

        private Image[,] squares;

        private V2? currentlyShowing = null;

        private List<V2> greyscaled = new List<V2>();

        private int[,] currentBoardHash;
        private int[,] currentBoardAppearance;

        private float transitionProgress = float.NaN;
        private enum Dimension { _2D, _3D }

        private Dimension targetDimension = Dimension._2D; // true = 2D
        [SerializeField] private float transitionTime = 2f;

        // Run once
        private void Awake()
        {
            // Populate sprite table
            foreach (var appearance_table in appearanceTables)
            {
                foreach (var piece_appearance in appearance_table.Appearances)
                {
#if UNITY_EDITOR
                    if (InternalAppearanceMap.ContainsKey(piece_appearance.ID))
                    {
                        throw new Exception("Duplicate appearance ID");
                    }
#endif

                    InternalAppearanceMap[piece_appearance.ID] = piece_appearance;
                }
            }

            // Load saved theme
            if (PlayerPrefs.HasKey(PLAYER_PREFS_THEME_KEY))
            {
                currentTheme = PlayerPrefs.GetInt(PLAYER_PREFS_THEME_KEY);
                if (currentTheme >= themes.Length) currentTheme = 0;
            }
        }

        // Run once
        void Start()
        {   
            // Get information for how board should be displayed
            boardRenderInfo = ChessManager.GameManager.Board.GetBoardRenderInfo();
            currentBoardHash = new int[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];
            currentBoardAppearance = new int[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];

            squares = new Image[boardRenderInfo.BoardSize, boardRenderInfo.BoardSize];

            // Initial update
            OnResolutionChange();
            RenderBoardBackground();
            if (boardRenderInfo.Allows3D)
            {
                VisualManager3D.RenderBoard(boardRenderInfo);
                VisualManager3D.UpdateTheme(themes[currentTheme]);
            }
            UpdateAllPieces();
            
        }

        /// <summary>
        /// Shows that a team has won
        /// </summary>
        /// <param name="team"></param>
        public void OnTeamWin(int team)
        {
            string team_string;

            // Set to team alias e.g. 'Black' if exists
            team_string = ChessManager.CurrentGameManager.TeamAliases().Length != 0 
                ? ChessManager.CurrentGameManager.TeamAliases()[team] : $"Team {team + 1}";

            teamWinText.text = $"{team_string} won!";
            teamWinText.gameObject.SetActive(true); // Show
        }

        /// <summary>
        /// Renders all pieces in BoardManager
        /// </summary>
        public void UpdateAllPieces(Move move) => UpdateAllPieces(move, true);

        /// <summary>
        /// Renders all pieces in BoardManager
        /// </summary>
        public void UpdateAllPieces() => UpdateAllPieces(new Move(new V2(-1, -1), new V2(-1, -1)), false);

        /// <summary>
        /// Renders all pieces in BoardManager
        /// </summary>
        private void UpdateAllPieces(Move move, bool hasMove)
        {
            void CheckSlide(V2 from, V2 to)
            {
                if (ChessManager.GameManager.Board.PieceBoard[to.X, to.Y] is not null &&
                    currentBoardHash[from.X, from.Y] == ChessManager.GameManager.Board.PieceBoard[to.X, to.Y].GetHashCode())
                {
                    // 2D Move
                    var piece = pieces2D[from];
                    piece.GetComponent<PieceController2D>().MoveTo(to, from, PieceTravelTime);

                    if (boardRenderInfo.Allows3D)
                    {
                        // 3D destroy if destination not empty
                        if (currentBoardHash[to.X, to.Y] != 0) VisualManager3D.DestroyPiece(to, targetDimension == Dimension._3D);
                        // 3D Move
                        VisualManager3D.Move(from, to);
                    }

                    // 2D Move
                    pieces2D.Remove(from);

                    // 2D destroy if destination not empty
                    if (currentBoardHash[to.X, to.Y] != 0)
                    {
                        currentBoardHash[to.X, to.Y] = 0;
                        Destroy(pieces2D[new V2(to.X, to.Y)]);
                        pieces2D.Remove(new V2(to.X, to.Y));
                    }

                    // 2D Move
                    pieces2D.Add(to, piece);

                    // Hash table updates
                    currentBoardHash[to.X, to.Y] = currentBoardHash[from.X, from.Y];
                    currentBoardHash[from.X, from.Y] = 0;
                    currentBoardAppearance[to.X, to.Y] = currentBoardAppearance[from.X, from.Y];
                    currentBoardAppearance[from.X, from.Y] = 0;
                }
            }

            // Check for sliding moves
            if (hasMove) CheckSlide(move.From, move.To);

            // Check for more sliding moves
            for (var x1 = 0; x1 < boardRenderInfo.BoardSize; x1++)
            {
                for (var y1 = 0; y1 < boardRenderInfo.BoardSize; y1++)
                {
                    // If empty continue
                    if (currentBoardHash[x1, y1] == 0 ||
                        currentBoardHash[x1, y1] == (ChessManager.GameManager.Board.PieceBoard[x1, y1]?.GetHashCode() ?? 0))
                        continue;

                    for (var x2 = 0; x2 < boardRenderInfo.BoardSize; x2++)
                    {
                        for (var y2 = 0; y2 < boardRenderInfo.BoardSize; y2++)
                        {
                            CheckSlide(new V2(x1, y1), new V2(x2, y2));
                        }
                    }
                }
            }

            // Resolve difference between previous and current board state
            for (var x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (var y = 0; y < boardRenderInfo.BoardSize; y++) {
                    if (currentBoardHash[x, y] != (ChessManager.GameManager.Board.PieceBoard[x, y]?.GetHashCode() ?? 0))
                    {
                        if (currentBoardHash[x, y] != 0)
                        {
                            // Destroy if already occupied
                            Destroy(pieces2D[new V2(x, y)]);
                            pieces2D.Remove(new V2(x, y));
                            if (boardRenderInfo.Allows3D) VisualManager3D.DestroyPiece(new V2(x, y), targetDimension == Dimension._3D);
                        }
                        if (ChessManager.GameManager.Board.PieceBoard[x, y] is not null)
                        {
                            // Place piece
                            AddPiece(ChessManager.GameManager.Board.PieceBoard[x, y]);
                            currentBoardAppearance[x, y] = ChessManager.GameManager.Board.PieceBoard[x, y].AppearanceID;
                            if (boardRenderInfo.Allows3D) VisualManager3D.Create(ChessManager.GameManager.Board.PieceBoard[x, y]);
                        }
                    }
                }
            }

            // Rebuild hash table
            for (var x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (var y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    if (ChessManager.GameManager.Board.PieceBoard[x, y] is null)
                    {
                        currentBoardHash[x, y] = 0;
                        currentBoardAppearance[x, y] = 0;
                    }
                    else
                    {
                        currentBoardHash[x, y] = ChessManager.GameManager.Board.PieceBoard[x, y].GetHashCode();

                        if (currentBoardAppearance[x, y] != ChessManager.GameManager.Board.PieceBoard[x, y].AppearanceID)
                        {
                            if (boardRenderInfo.Allows3D) VisualManager3D.UpdateAppearance(new V2(x, y));
                            UpdatePiece(ChessManager.GameManager.Board.PieceBoard[x, y]);
                        }
                        currentBoardAppearance[x, y] = ChessManager.GameManager.Board.PieceBoard[x, y].AppearanceID;
                    }
                }
            }
        }

        /// <summary>
        /// Renders a specific piece
        /// </summary>
        /// <param name="piece"></param>
        private void AddPiece(AbstractPiece piece)
        {
            // Duplicate piece
            var new_gameobject = Instantiate(piecePrefab);
            pieces2D.Add(piece.Position, new_gameobject);
            // var image = new_gameobject.GetComponent<Image>();

            if (boardRenderInfo.Allows3D)
            {
                // new_gameobject = Instantiate(internalAppearanceMap[piece.AppearanceID].Prefab3D);
            }

            // Show
            new_gameobject.SetActive(true);

            UpdatePiece(piece);
        }

        /// <summary>
        /// Updates the visuals of a specific piece
        /// </summary>
        /// <param name="piece"></param>
        private void UpdatePiece(AbstractPiece piece)
        {
            var image = pieces2D[piece.Position].GetComponent<Image>();
            image.sprite = InternalAppearanceMap[piece.AppearanceID].Sprite; // Get piece appearance

            SizeGameObject(image.gameObject, piece.Position); // Resize for display scale
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
        public static bool IsWhite(V2 position) => (position.X + position.Y) % 2 == 0;

        /// <summary>
        /// Renders the board's cells
        /// </summary>
        private void RenderBoardBackground()
        {
            for (var x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (var y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    var new_square = Instantiate(squarePrefab);

                    SizeGameObject(new_square, new V2(x, y)); // Size for board

                    var rendered_piece_data = new_square.GetComponent<RenderedCellData>();
                    rendered_piece_data.Position = new V2(x, y); // Set position data

                    var image = new_square.GetComponent<Image>();
                    squares[x, y] = image;

                    ResetSquareColor(new V2(x, y)); // Set to default colour

                    new_square.SetActive(true); // Show
                }
            }
        }

        // ReSharper disable once ParameterHidesMember
        public void SizeGameObject(GameObject gameObject, V2 position) => SizeGameObject(gameObject, position.Vector2());

        // Gives a GameObject the correct size, scale and position
        // ReSharper disable once ParameterHidesMember
        public void SizeGameObject(GameObject gameObject, Vector2 position)
        {
            var rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(renderBox); // Set as a child of the render box
            
            // Set min and max anchors to corners of square
            rect.anchorMin = new Vector2(position.x / boardRenderInfo.BoardSize, position.y / boardRenderInfo.BoardSize);
            rect.anchorMax = new Vector2((position.x + 1) / boardRenderInfo.BoardSize, (position.y + 1) / boardRenderInfo.BoardSize);

            // Set position to that square
            var render_box_rect = renderBox.rect;
            rect.localPosition = new Vector2((position.x + 0.5f) * (render_box_rect.width / boardRenderInfo.BoardSize) - (render_box_rect.width / 2),
                (position.y + 0.5f) * (render_box_rect.width / boardRenderInfo.BoardSize) - (render_box_rect.width / 2));

            // Resize
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, render_box_rect.width / boardRenderInfo.BoardSize);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, renderBox.rect.width / boardRenderInfo.BoardSize);
        }

        /// <summary>
        /// Updates the VisualManager's internal list of possible moves
        /// </summary>
        /// <param name="possibleMoves"></param>
        public void SetPossibleMoves(List<Move> possibleMoves)
        {
            this.PossibleMoves = possibleMoves;
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

            foreach (var move in PossibleMoves)
            {
                if (move.From == clickPosition)
                {
                    // Create new move indicator

                    var new_indicator = Instantiate(moveOptionPrefab);
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
            // Set colour to themed select colour
            if (IsWhite(position)) squares[position.X, position.Y].color = themes[currentTheme].WhiteSelectColor;
            else squares[position.X, position.Y].color = themes[currentTheme].BlackSelectColor;
        }

        /// <summary>
        /// Marks a square as having had a piece move there last turn
        /// </summary>
        /// <param name="position"></param>
        public void ShowPreviousMove(V2 position)
        {
            if (IsWhite(position)) squares[position.X, position.Y].color = themes[currentTheme].WhiteMoveColor;
            else squares[position.X, position.Y].color = themes[currentTheme].BlackMoveColor;
        }

        /// <summary>
        /// Resets a square back to its default colour
        /// </summary>
        /// <param name="position"></param>
        public void ResetSquareColor(V2 position)
        {
            var alpha = squares[position.X, position.Y].color.a;
            if (IsWhite(position))
            {
                if (boardRenderInfo.RemovedSquares.Contains(position))
                {
                    squares[position.X, position.Y].color = themes[currentTheme].WhiteBlockedColor;
                }
                else if (boardRenderInfo.HighlightedSquares.Contains(position))
                {
                    squares[position.X, position.Y].color = themes[currentTheme].WhiteHighlightColor;
                }
                else
                {
                    squares[position.X, position.Y].color = themes[currentTheme].WhiteColor;
                }
            }
            else
            {
                if (boardRenderInfo.RemovedSquares.Contains(position))
                {
                    squares[position.X, position.Y].color = themes[currentTheme].BlackBlockedColor;
                }
                else if (boardRenderInfo.HighlightedSquares.Contains(position))
                {
                    squares[position.X, position.Y].color = themes[currentTheme].BlackHighlightColor;
                }
                else
                {
                    squares[position.X, position.Y].color = themes[currentTheme].BlackColor;
                }
            }
            var square_color = squares[position.X, position.Y].color;
            square_color.a = alpha;
            squares[position.X, position.Y].color = square_color;
        }

        /// <summary>
        /// Shows which piece moved on the last turn
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void OnMove(V2 from, V2 to)
        {
            foreach (var grey in greyscaled) ResetSquareColor(grey); // Reset grey from prefious turn
            greyscaled.Clear();

            // Show new move
            ShowPreviousMove(from);
            ShowPreviousMove(to);
            greyscaled.Add(from);
            greyscaled.Add(to);

            if (boardRenderInfo.Allows3D) VisualManager3D.ShowLastMove(from, to);
        }

        /// <summary>
        /// Updates the turn text on turn change
        /// </summary>
        /// <param name="team"></param>
        /// <param name="playerOnTeam"></param>
        /// <param name="you"></param>
        /// <param name="ai"></param>
        public void OnTurn(int team, int playerOnTeam, bool you, bool ai = false)
        {
            // Shows team alias if exists
            string team_string;
            team_string = ChessManager.CurrentGameManager.TeamAliases().Length != 0 ? 
                ChessManager.CurrentGameManager.TeamAliases()[team] : $"T{team+1}";

            var str = $"Turn: {team_string}, P{playerOnTeam+1}";
            if (ai) str += " (AI)";
            else if (you) str += " (You)";
            turnText.text = str;
        }

        /// <summary>
        /// Updates AI progress text
        /// </summary>
        /// <param name="waiting"></param>
        /// <param name="progress"></param>
        public void ShowAIInfo(bool waiting, float progress)
        {
            if (!waiting) aiText.gameObject.SetActive(false); // Hide if inactive
            else
            {
                aiText.gameObject.SetActive(true);
                aiText.text = $"AI: {Mathf.Round(progress)}%";
            }
        }

        /// <summary>
        /// Updates the squares to the current theme colour
        /// </summary>
        private void UpdateTheme() 
        {
            // Reset all squares
            for (var x = 0; x < boardRenderInfo.BoardSize; x++)
            {
                for (var y = 0; y < boardRenderInfo.BoardSize; y++)
                {
                    ResetSquareColor(new V2(x, y));
                }
            }

            if (boardRenderInfo.Allows3D)
                VisualManager3D.UpdateTheme(themes[currentTheme]);
        }

        /// <summary>
        /// Cycles the current theme and saves the new theme
        /// </summary>
        public void CycleTheme()
        {
            currentTheme++;
            if (currentTheme >= themes.Length) currentTheme = 0;
            UpdateTheme();
            PlayerPrefs.SetInt(PLAYER_PREFS_THEME_KEY, currentTheme); // Store theme
        }

        private void Update()
        {
            // Check for theme change input
            if (I.GetKeyDown(K.ChangeThemeKey) && !GameMenuManager.ShowingEscapeMenu)
            {
                CycleTheme();
            }

            // Transition between 2D and 3D
            if (I.GetKeyDown(K.ChangeDimensionKey) && transitionProgress is float.NaN && boardRenderInfo.Allows3D)
            {
                targetDimension = targetDimension == Dimension._2D ? Dimension._3D : Dimension._2D;

                transitionProgress = 0f;
                if (targetDimension == Dimension._2D) renderBox.gameObject.SetActive(true);
            }

            if (transitionProgress is not float.NaN)
            {
                transitionProgress += Time.deltaTime / transitionTime;
                if (transitionProgress >= 1f) transitionProgress = 1f;

                var progress = transitionProgress;
                if (targetDimension == Dimension._3D) progress = 1f - progress;

                progress = MathP.CosSmooth(progress);

                hide3DImage.color = new Color(hide3DImage.color.r, hide3DImage.color.g, hide3DImage.color.b, progress);

                var children = renderBox.transform.GetComponentsInChildren<Image>();
                Color new_colour;
                foreach (var child in children)
                {
                    new_colour = child.color;
                    new_colour.a = progress;
                    child.color = new_colour;
                }

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (transitionProgress == 1f)
                {
                    transitionProgress = float.NaN;
                    if (targetDimension == Dimension._3D)
                    {
                        renderBox.gameObject.SetActive(false);
                        VisualManager3D.ClearRipples();
                    }
                }
            }

            if (targetDimension == Dimension._3D)
            {
                VisualManager3D.ExternalUpdate();
            }

            // Check for resolution change
            if (resolution.height != Screen.height || resolution.width != Screen.width) OnResolutionChange();
            
            // Timer
            // TODO: Make this not run every frame
            var time = ChessManager.Timer.ElapsedMilliseconds + ChessManager.TimerOffset;
            var hours = time / (60 * 60 * 1000);
            time -= hours * (60 * 60 * 1000);
            var minutes = time / (60 * 1000);
            time -= minutes * (60 * 1000);
            var seconds = time / (1000);

            var time_string = $"{seconds}";
            while (time_string.Length < 2) time_string = "0" + time_string;
            time_string = ":" + time_string;
            time_string = minutes + time_string;
            while (time_string.Length < 5) time_string = "0" + time_string;
            time_string = ":" + time_string;
            time_string = hours + time_string;
            while (time_string.Length < 8) time_string = "0" + time_string;
            timerText.text = time_string;
        }
    }
}