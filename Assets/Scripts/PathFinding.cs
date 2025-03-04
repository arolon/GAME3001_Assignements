using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public GridManager gridManager;
    private List<Tile> openList, closedList;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            ToggleDebugView();
        }
    }

    void ToggleDebugView()
    {
        Debug.Log("DEBUG TOOGLE");
        foreach (Tile tile in gridManager.grid)
        {
            tile.costText.enabled = !tile.costText.enabled;
        }
    }

    public List<Tile> FindPath(Tile start, Tile goal)
    {
        openList = new List<Tile> { start };
        closedList = new List<Tile>();

        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, int> gScore = new Dictionary<Tile, int> { { start, 0 } };
        Dictionary<Tile, int> fScore = new Dictionary<Tile, int> { { start, GetHeuristic(start, goal) } };

        while (openList.Count > 0)
        {
            Tile current = GetLowestFScore(fScore);

            if (current == goal)
                return ReconstructPath(cameFrom, goal);

            openList.Remove(current);
            closedList.Add(current);

            foreach (Tile neighbor in GetNeighbors(current))
            {
                if (neighbor.isObstacle || closedList.Contains(neighbor))
                    continue;

                int tentativeGScore = gScore[current] + neighbor.cost;

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + GetHeuristic(neighbor, goal);
            }
        }

        return null; // No path found
    }

    int GetHeuristic(Tile a, Tile b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan Distance
    }

    Tile GetLowestFScore(Dictionary<Tile, int> fScore)
    {
        Tile lowest = null;
        int minScore = int.MaxValue;

        foreach (Tile tile in openList)
        {
            if (fScore.ContainsKey(tile) && fScore[tile] < minScore)
            {
                minScore = fScore[tile];
                lowest = tile;
            }
        }
        return lowest;
    }

    List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        int[][] directions = {
            new int[] { 0, 1 },  // Up
            new int[] { 1, 0 },  // Right
            new int[] { 0, -1 }, // Down
            new int[] { -1, 0 }  // Left
        };

        foreach (int[] dir in directions)  // Now directions is a valid jagged array
        {
            int newX = tile.x + dir[0];
            int newY = tile.y + dir[1];

            if (newX >= 0 && newX < gridManager.width && newY >= 0 && newY < gridManager.height)
            {
                neighbors.Add(gridManager.grid[newX, newY]);
            }
        }

        return neighbors;
    }

    List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        List<Tile> path = new List<Tile> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}
