using Unity.VisualScripting;
using UnityEngine;

public class BoardFactory : MonoBehaviour
{
    [Header("Grid Parameters")]
    [Range(1,50)]
    public int boardWidth; 

    [Range(1,50)]
    public int boardHeight;

    public static int width;
    public static int height; 

    [Range(1, 50)]
    public float cellWidth;

    [Range(1,50)]
    public float cellHeight;
    public GameObject cell;
    public GameObject runner;
    private GameObject[,] grid;

    void createGrid()
    {
        grid = new GameObject[boardWidth, boardHeight];

        Vector2 startPoint = calculateStartPoint();

        float startX = startPoint.x;
        float startY = startPoint.y;

        Vector3 cellScale = new Vector3(cellWidth, cellHeight, 1);
        Vector3 instantiationPoint = new Vector3(startX, startY, 0);

        for (int i = 0; i < boardHeight; i++)
        {

            for (int j = 0; j < boardWidth; j++)
            {
                GameObject currentCell = Instantiate(cell, instantiationPoint, Quaternion.identity);
                CellController.cellCount++;
                currentCell.transform.localScale = cellScale;
                instantiationPoint.x = instantiationPoint.x + cellWidth;
                grid[j, i] = currentCell;
            }

            instantiationPoint.y = instantiationPoint.y - cellHeight;
            instantiationPoint.x = startX;
        }

        grid[0, boardHeight - 1].GetComponent<CellController>().setStart();
        grid[boardWidth - 1, 0].GetComponent<CellController>().setEnd();
        runner.GetComponent<AStarAlgo>().setup(grid, new AStarAlgo.Pair(0, boardHeight - 1), new AStarAlgo.Pair(boardWidth - 1, 0));
    }

    Vector2 calculateStartPoint() {
        return new Vector2(-(cellWidth * boardWidth/2), cellHeight * boardHeight/2);
    }

    void Start() {
        height = boardHeight;
        width = boardWidth;
        createGrid();
    }

    void Update() {
        
    }

}