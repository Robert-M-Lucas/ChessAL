using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour
{
    public Sprite[] PieceSprites;

    public RectTransform renderBox;

    public GameObject BlackSquare;
    public GameObject WhiteSquare;
    public GameObject BlockedSquare;
    public GameObject PiecePrefab;
    public GameObject MoveOptionPrefab;

    private ChessManager chessManager;
    private Resolution resolution = new Resolution();

    private Dictionary<AbstractPiece, Image> piece_images = new Dictionary<AbstractPiece, Image>();

    private BoardRenderInfo boardRenderInfo;

    private List<Move> possibleMoves = new List<Move>();
    private List<GameObject> moveIndicators = new List<GameObject>();

    private V2? currentlyShowing = null;

    void Start()
    {
        chessManager = FindObjectOfType<ChessManager>();
        OnResolutionChange();
        // Render(new SampleBoard().GetBoardRenderInfo());
        boardRenderInfo = chessManager.GameManager.Board.GetBoardRenderInfo();
        RenderBoardBackground();
        AddAllPieces();
    }

    /// <summary>
    /// Renders all pieces in BoardManager
    /// </summary>
    private void AddAllPieces()
    {
        for (int x = 0; x < boardRenderInfo.BoardSize; x++)
        {
            for (int y = 0; y < boardRenderInfo.BoardSize; y++)
            {
                if (chessManager.GameManager.Board.PieceBoard[x,y] is not null) AddPiece(chessManager.GameManager.Board.PieceBoard[x,y]);
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
        Image image = new_gameobject.GetComponent<Image>();
        RectTransform rect = new_gameobject.GetComponent<RectTransform>();

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
        image.sprite = PieceSprites[piece.AppearanceID];

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
        Vector2 centre = new Vector2(renderBox.rect.width / 2, renderBox.rect.height / 2);

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
                    if ((x + y) % 2 == 0) square = WhiteSquare;
                    else square = BlackSquare;
                }

                GameObject new_square = Instantiate(square);
                SizeGameObject(new_square, new V2(x, y));
                RenderedCellData rendered_piece_data = new_square.GetComponent<RenderedCellData>();
                rendered_piece_data.Position = new V2(x, y);
                new_square.SetActive(true);
            }
        }
    }

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

    void Update()
    {
        // Check for resolution change
        if (resolution.height != Screen.height || resolution.width != Screen.width) OnResolutionChange();
    }
    
    /// <summary>
    /// Updates the VisualManager's internal list of possible moves
    /// </summary>
    /// <param name="possibleMoves"></param>
    public void SetPossibleMoves(List<Move> possibleMoves)
    {
        this.possibleMoves = possibleMoves;
    }

    public void ToggleShowMoves(V2 clickPosition)
    {
        while (moveIndicators.Count > 0)
        {
            Destroy(moveIndicators[0]);
            moveIndicators.RemoveAt(0);
        }

        if (chessManager.GameManager.Board.PieceBoard[clickPosition.X, clickPosition.Y] is null)
        {
            currentlyShowing = null;
            return;
        }

        if (currentlyShowing is not null && ((V2)currentlyShowing).X == clickPosition.X && ((V2)currentlyShowing).Y == clickPosition.Y)
        {
            currentlyShowing = null;
            return;
        }

        currentlyShowing = clickPosition;

        foreach (Move move in possibleMoves)
        {
            if (move.From.X == clickPosition.X && move.From.Y == clickPosition.Y)
            {
                GameObject new_indicator = Instantiate(MoveOptionPrefab);
                SizeGameObject(new_indicator, move.To);
                new_indicator.SetActive(true);
                moveIndicators.Add(new_indicator);
            }
        }
    }
}
