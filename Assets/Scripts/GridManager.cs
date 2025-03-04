using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 10, height = 10;
    public GameObject tilePrefab; // Assign the Tile prefab in Unity
    public Tile[,] grid;
    Tile startTile, goalTile;
    public ActorController actor;
    public bool debugMode = false;
    private List<Tile> currentPath = new List<Tile>();

    void Start()
    {
        GenerateGrid();
        Camera.main.transform.position = new Vector3(width / 2f - 0.5f, height / 2f - 0.5f, -10f);
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) // Left Click
        //{
        //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Tile clickedTile = Physics2D.OverlapPoint(mousePos)?.GetComponent<Tile>();

        //    if (clickedTile != null && !clickedTile.isObstacle)
        //    {
        //        startTile = clickedTile;
        //    }
        //}

        //if (Input.GetMouseButtonDown(1)) // Right Click
        //{
        //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Tile clickedTile = Physics2D.OverlapPoint(mousePos)?.GetComponent<Tile>();

        //    if (clickedTile != null && !clickedTile.isObstacle && clickedTile != startTile)
        //    {
        //        goalTile = clickedTile;
        //        FindPath();
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleDebugMode();
        }

        if (!debugMode) return; // Restrict tile selection to Debug Mode

        if (Input.GetMouseButtonDown(0)) // Left Click (Select Start Tile)
        {
            SelectTile(true);
        }

        if (Input.GetMouseButtonDown(1)) // Right Click (Select Goal Tile)
        {
            SelectTile(false);
        }
    }

    void ToggleDebugMode()
    {
        debugMode = !debugMode;

        foreach (Tile tile in grid)
        {
            tile.ToggleDebug(debugMode); // Show/hide debug text
        }
    }

    void SelectTile(bool isStart)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Tile clickedTile = Physics2D.OverlapPoint(mousePos)?.GetComponent<Tile>();

        if (clickedTile != null && !clickedTile.isObstacle)
        {
            if (isStart)
            {
                if (startTile != null) startTile.ResetColor(); // Reset old start tile
                startTile = clickedTile;
                startTile.SetColor(Color.blue); // Set start tile color
            }
            else if (clickedTile != startTile) // Goal cannot be the same as start
            {
                if (goalTile != null) goalTile.ResetColor(); // Reset old goal tile
                goalTile = clickedTile;
                goalTile.SetColor(Color.red); // Set goal tile color

                FindPath(); // Find new path immediately after setting goal
            }
        }
    }

    void FindPath()
    {
        if (startTile == null || goalTile == null) return; // Both tiles must be set

        PathFinding pathfinder = GetComponent<PathFinding>();
        List<Tile> path = pathfinder.FindPath(startTile, goalTile);

        ClearPreviousPath(); // Clear old path before drawing new one

        if (path != null)
        {
            foreach (Tile tile in path)
            {
                tile.SetColor(Color.green); // Highlight new path
            }
            currentPath = path;
            actor.SetPath(path);
        }
    }

    void ClearPreviousPath()
    {
        foreach (Tile tile in currentPath)
        {
            tile.ResetColor();
        }
        currentPath.Clear();
    }


    void GenerateGrid()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                Tile tile = newTile.GetComponent<Tile>();

                // Randomly place obstacles (20% chance)
                bool isObstacle = Random.value < 0.2f;
                tile.SetTileData(x, y, isObstacle);

                grid[x, y] = tile;
            }
        }
    }
}
