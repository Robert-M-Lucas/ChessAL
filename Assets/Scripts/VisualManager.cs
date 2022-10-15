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

    private ChessManager chessManager;
    private Resolution resolution = new Resolution();

    private Dictionary<AbstractPiece, Image> piece_images = new Dictionary<AbstractPiece, Image>();

    private BoardRenderInfo boardRenderInfo;

    private List<Move> possibleMoves = new List<Move>();

    void Start()
    {
        chessManager = FindObjectOfType<ChessManager>();
        OnResolutionChange();
        // Render(new SampleBoard().GetBoardRenderInfo());
        boardRenderInfo = chessManager.gameManager.Board.GetBoardRenderInfo();
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
                if (chessManager.gameManager.Board.PieceBoard[x,y] is not null) AddPiece(chessManager.gameManager.Board.PieceBoard[x,y]);
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

        rect.SetParent(renderBox);
        rect.anchorMin = new Vector2((float)piece.Position.X / boardRenderInfo.BoardSize, (float)piece.Position.Y / boardRenderInfo.BoardSize);
        rect.anchorMax = new Vector2((float)(piece.Position.X + 1) / boardRenderInfo.BoardSize, (float)(piece.Position.Y + 1) / boardRenderInfo.BoardSize);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, renderBox.rect.width / boardRenderInfo.BoardSize);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, renderBox.rect.width / boardRenderInfo.BoardSize);

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

        RectTransform rect = image.gameObject.GetComponent<RectTransform>();
        rect.localPosition = new Vector2((piece.Position.X + 0.5f) * (renderBox.rect.width / boardRenderInfo.BoardSize) - (renderBox.rect.width / 2),
                    (piece.Position.Y + 0.5f) * (renderBox.rect.width / boardRenderInfo.BoardSize) - (renderBox.rect.width / 2));
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
                RectTransform rect = new_square.GetComponent<RectTransform>();
                rect.SetParent(renderBox);
                rect.anchorMin = new Vector2((float)x / boardRenderInfo.BoardSize, (float)y / boardRenderInfo.BoardSize);
                rect.anchorMax = new Vector2((float)(x + 1) / boardRenderInfo.BoardSize, (float)(y + 1) / boardRenderInfo.BoardSize);
                rect.localPosition = new Vector2((x + 0.5f) * (renderBox.rect.width / boardRenderInfo.BoardSize) - (renderBox.rect.width / 2), 
                    (y + 0.5f) * (renderBox.rect.width / boardRenderInfo.BoardSize) - (renderBox.rect.width / 2));
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, renderBox.rect.width / boardRenderInfo.BoardSize);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, renderBox.rect.width / boardRenderInfo.BoardSize);
                new_square.SetActive(true);
            }
        }
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
}
