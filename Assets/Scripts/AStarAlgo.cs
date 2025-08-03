using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class AStarAlgo : MonoBehaviour
{
    public class Pair
    {
        public int first, second;
        public float fValue, gValue;
        public Pair parent;

        public Pair(int x, int y)
        {
            first = x;
            second = y;
            fValue = float.MaxValue;
            gValue = 0;
            parent = null;
        }

        public bool isEqual(Pair p)
        {
            if (p.first == this.first && p.second == this.second)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private GameObject[,] grid;
    private Pair startPoint;
    private Pair solutionPair;
    private Pair endPoint;
    private bool foundEnd;

    private float bestScore = -1;
    public static bool boardChanged = true;


    private List<Pair> openList = new List<Pair>();
    private List<Pair> closedList = new List<Pair>();
    private Dictionary<(int, int), Pair> allPairs = new();

    private Pair solution = new Pair(-1, -1);

    public void setup(GameObject[,] grid, Pair startPoint, Pair endPoint)
    {

        this.grid = grid;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
    }

    public void run()
    {
        Debug.LogWarning("SELF REFERENTIAL LOOP, NEVER TERMINATES NATURALLY");
        foreach (GameObject g in grid)
        {
            if (g.GetComponent<CellController>().getState() == CellController.CellState.solution)
            {
                g.GetComponent<CellController>().setBlank();
            }
        }

        openList.Clear();
        closedList.Clear();
        allPairs.Clear();
        foundEnd = false;


        startPoint.gValue = 0;
        startPoint.fValue = calculateF(startPoint, startPoint);

        openList.Add(startPoint);

        solution = AStarSearch(grid, startPoint, endPoint);

        Pair solutionCopy = solution.parent;
        if (bestScore == -1)
        {
            bestScore = solutionCopy.fValue;
        }

        if (solutionCopy == null)
        {
            Debug.LogWarning("Solution was null, exiting");
            return;
        }

        while (solutionCopy.parent != null && foundEnd)
        {
            grid[solutionCopy.first, solutionCopy.second].GetComponent<CellController>().setSolution(solutionCopy.fValue, bestScore);
            solutionCopy = solutionCopy.parent;
        }

    }

    public void ResetGrid()
    {
        foreach (GameObject g in grid)
        {
            if (g.GetComponent<CellController>().getState() == CellController.CellState.wall)
            {
                g.GetComponent<CellController>().setBlank();
            }

        }
        run();
    }

    private Pair AStarSearch(GameObject[,] grid, Pair startPoint, Pair endPoint)
    {
        float lowestFValue;
        Pair candidate = new Pair(-1, -1);

        // Picking the start node, i.e. the one with the lowest f value
        while (openList.Count > 0 && foundEnd == false)
        {
            lowestFValue = int.MaxValue;
            foreach (Pair p in openList)
            {
                if (p.fValue < lowestFValue)
                {
                    lowestFValue = p.fValue;
                    candidate = p;
                }
            }
            if (lowestFValue == float.MaxValue)
            {
                Debug.LogWarning("All remaining nodes have infinite cost. No path found.");
                break;
            }
            if (candidate.fValue != int.MaxValue)
            {
                openList.Remove(candidate);
            }
            // Take candidate move, generate all next moves for it
            calculateNextStep(candidate);
            closedList.Add(candidate);

        }
        return solutionPair;
    }

    private void calculateNextStep(Pair q)
    {
        foreach (Pair successor in generateSuccessors(q))
        {
            if (successor.isEqual(endPoint))
            {
                solutionPair = successor;
                foundEnd = true;
                break;
            }
            else
            {
                successor.fValue = calculateF(successor, q);

                Pair existing = openList.FirstOrDefault(p => p.first == successor.first && p.second == successor.second);

                if (existing != null)
                {
                    if (existing.fValue > successor.fValue)
                    {
                        existing.fValue = successor.fValue;
                        existing.gValue = successor.gValue;
                        existing.parent = q;
                    }
                    continue;
                }

                existing = closedList.FirstOrDefault(p => p.first == successor.first && p.second == successor.second);

                if (existing != null)
                {
                    if (existing.fValue > successor.fValue)
                    {
                        existing.fValue = successor.fValue;
                        existing.gValue = successor.gValue;
                        existing.parent = q;
                    }
                    continue;
                }

                openList.Add(successor);
            }
        }
    }

    private Pair[] generateSuccessors(Pair q)
    {
        List<Pair> successorsList = new List<Pair>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int newX = q.first + i;
                int newY = q.second + j;

                if (!cellIsValid(newX, newY))
                {
                    continue;
                }

                // Prevent diagonal corner cutting
                if (i != 0 && j != 0)
                {
                    if (!cellIsValid(q.first + i, q.second) || !cellIsValid(q.first, q.second + j))
                    {
                        continue;
                    }
                }

                if (!allPairs.TryGetValue((newX, newY), out Pair successor))
                {
                    successor = new Pair(newX, newY);
                    allPairs[(newX, newY)] = successor;
                    Debug.Log("setting parent node to " + q.first + ", " + q.second);
                    successor.parent = q;
                }

                successorsList.Add(successor);
            }
        }

        return successorsList.ToArray();
    }

    private float calculateF(Pair current, Pair parent)
    {

        float dx = Mathf.Abs(current.first - endPoint.first);
        float dy = Mathf.Abs(current.second - endPoint.second);
        float D = 1f;
        float D2 = Mathf.Sqrt(2);
        float h = D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);

        float g;
        if (parent.first == current.first || parent.second == current.second)
        {
            g = parent.gValue + 1;
        }
        else
        {
            g = parent.gValue + Mathf.Sqrt(2);
        }

        current.gValue = g;

        float epsilon = 1.0f / 1000.0f;
        return g + h * (1.0f + epsilon);
    }

    private bool cellIsValid(int i, int j)
    {
        if (i >= 0 && i < BoardFactory.width && j >= 0 && j < BoardFactory.height)
        {
            return grid[i, j].GetComponent<CellController>().getState() != CellController.CellState.wall;
        }
        return false;
    }

    void Update()
    {
        if (boardChanged)
        {
            run();
            boardChanged = false;
        }
    }
}
