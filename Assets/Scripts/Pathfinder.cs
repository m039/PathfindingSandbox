using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Node
    {
        public int f => g + h;
        public int h;
        public int g;

        public readonly int x;
        public readonly int y;

        public readonly GridCell cell;

        public Node(GridCell cell)
        {
            x = cell.x;
            y = cell.y;
            this.cell = cell;
        }

        public void UpdateCell()
        {
            cell.h = h;
            cell.g = g;
            cell.f = f;
            cell.SetTextVisibility(true);
        }
    }

    public class Pathfinder
    {
        readonly GridView _gridView;

        public Pathfinder(GridView gridView)
        {
            _gridView = gridView;
        }

        const float Delay = 0.1f;

        public IEnumerator Search(GridCell start, GridCell goal)
        {
            ResetCellsState();

            var startNode = new Node(start);
            var goalNode = new Node(goal);
            startNode.g = 0;
            startNode.h = CalculateHeuristic(startNode.x, startNode.y, goalNode.x, goalNode.y);
            startNode.UpdateCell();

            var toSearch = new Dictionary<GridCell, Node>() { { start, startNode } };
            var opened = new Dictionary<GridCell, Node>();

            while (true)
            {
                Node bestF = null;
                foreach (var n in toSearch.Values)
                {
                    if (bestF == null || bestF.f > n.f || (bestF.f == n.f && bestF.h > n.h))
                    {
                        bestF = n;
                    }
                }

                if (bestF == null)
                    break;

                if (bestF.cell == goal)
                    break;

                toSearch.Remove(bestF.cell);
                opened.Add(bestF.cell, bestF);

                SetState(bestF, GridCell.CellState.Opened);

                foreach (var neighbor in GetNeighbors(bestF))
                {
                    if (!toSearch.ContainsKey(neighbor.cell) && !opened.ContainsKey(neighbor.cell)) {
                        neighbor.g = GetG(bestF, neighbor);
                        neighbor.h = CalculateHeuristic(neighbor.x, neighbor.y, goal.x, goal.y);
                        SetState(neighbor, GridCell.CellState.Frontier);
                        neighbor.UpdateCell();

                        toSearch.Add(neighbor.cell, neighbor);
                    }
                }

                yield return new WaitForSeconds(Delay);
            }
        }

        static void SetState(Node node, GridCell.CellState state)
        {
            var s = node.cell.GetState();
            if (s != GridCell.CellState.StartNode && s != GridCell.CellState.GoalNode)
            {
                node.cell.SetState(state);
            }
        }

        static Vector2Int[] Directions = new[]
        {
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
        };

        static int GetG(Node current, Node neighbor)
        {
            return current.g + CalculateHeuristic(current.x, current.y, neighbor.x, neighbor.y);
        }

        List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            foreach (var direction in Directions)
            {
                var x = node.x + direction.x;
                var y = node.y + direction.y;
                if (x >= 0 && x < _gridView.Cells.GetLength(0) - 1 &&
                    y >= 0 && y < _gridView.Cells.GetLength(1) - 1)
                {
                    neighbors.Add(new Node(_gridView.Cells[x, y]));
                }
            }

            return neighbors;
        }

        void ResetCellsState()
        {
            for (int x = 0; x < _gridView.Cells.GetLength(0); x++)
            {
                for (int y = 0; y < _gridView.Cells.GetLength(1); y++)
                {
                    var cell = _gridView.Cells[x, y];
                    cell.SetTextVisibility(false);

                    var state = cell.GetState();
                    if (state == GridCell.CellState.StartNode ||
                        state == GridCell.CellState.GoalNode)
                        continue;

                    cell.SetState(GridCell.CellState.Empty);
                }
            }
        }

        static int CalculateHeuristic(int x1, int y1, int x2, int y2)
        {
            var dx = Mathf.Abs(x2 - x1);
            var dy = Mathf.Abs(y2 - y1);
            var diagonal = Mathf.Min(dx, dy);
            var straight = Mathf.Max(dx, dy) - diagonal;
            return diagonal * 14 + straight * 10;
        }
    }
}
