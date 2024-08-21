using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game
{
    public enum NodeState
    {
        Openned, Blocked
    }

    public class Node
    {
        public NodeState state = NodeState.Openned;

        public int f => g + h;
        public int h;
        public int g;

        public readonly int x;
        public readonly int y;

        public readonly GridCell cell;

        public Node connection;

        public Node(int x, int y, GridCell cell)
        {
            this.x = x;
            this.y = y;
            this.cell = cell;
        }
    }

    public class Pathfinder
    {
        readonly GridView _gridView;

        readonly GameController _gameController;

        public Pathfinder(GameController gameController, GridView gridView)
        {
            _gameController = gameController;
            _gridView = gridView;
        }

        const float Delay = 0.1f;

        Node _startNode;

        Node _goalNode;
        public List<Node> path { get; private set; }

        List<Node> GetPath(Node node)
        {
            var result = new List<Node>() { node };
            var n = node;
            while (true)
            {
                n = n.connection;
                result.Add(n);

                if (n == null)
                    break;

                if (n == _startNode)
                {
                    result.Reverse();
                    return result;
                }
            }

            return null;
        }

        public IEnumerator Search(Node startNode, Node goalNode)
        {
            path = null;
            _startNode = startNode;
            _goalNode = goalNode;

            ResetCellsState();

            startNode.connection = null;
            startNode.g = 0;
            startNode.h = CalculateHeuristic(startNode, goalNode);
            UpdateText(startNode);

            var toSearch = new HashSet<Node>() { startNode };
            var opened = new HashSet<Node>();

            while (true)
            {
                Node bestF = null;
                foreach (var n in toSearch)
                {
                    if (bestF == null || bestF.f > n.f || (bestF.f == n.f && bestF.h > n.h))
                    {
                        bestF = n;
                    }
                }

                if (bestF == null)
                    break;

                _gameController.DrawPath(GetPath(bestF));

                if (bestF == goalNode)
                {
                    path = GetPath(bestF);
                    break;
                }

                toSearch.Remove(bestF);
                opened.Add(bestF);

                SetState(bestF, GridCell.CellState.Opened);

                foreach (var neighbor in GetNeighbors(bestF))
                {
                    if (!toSearch.Contains(neighbor) && !opened.Contains(neighbor) && neighbor.state == NodeState.Openned) {
                        neighbor.g = bestF.g + CalculateHeuristic(bestF, neighbor);
                        neighbor.h = CalculateHeuristic(neighbor, goalNode);
                        SetState(neighbor, GridCell.CellState.Frontier);
                        UpdateText(neighbor);

                        toSearch.Add(neighbor);

                        neighbor.connection = bestF;
                    }
                }

                yield return new WaitForSeconds(Delay);
            }
        }

        static void UpdateText(Node node)
        {
            var cell = node.cell;
            cell.h = node.h;
            cell.g = node.g;
            cell.f = node.f;
            cell.SetTextVisibility(true);
        }

        void SetState(Node node, GridCell.CellState state)
        {
            if (node != _startNode && node != _goalNode)
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

        List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            foreach (var direction in Directions)
            {
                var x = node.x + direction.x;
                var y = node.y + direction.y;
                if (x >= 0 && x < _gridView.Nodes.GetLength(0) &&
                    y >= 0 && y < _gridView.Nodes.GetLength(1))
                {
                    neighbors.Add(_gridView.Nodes[x, y]);
                }
            }

            return neighbors;
        }

        void ResetCellsState()
        {
            for (int x = 0; x < _gridView.Nodes.GetLength(0); x++)
            {
                for (int y = 0; y < _gridView.Nodes.GetLength(1); y++)
                {
                    var node = _gridView.Nodes[x, y];
                    var cell = node.cell;

                    cell.SetTextVisibility(false);

                    if (node == _startNode ||
                        node == _goalNode)
                        continue;

                    if (node.state == NodeState.Openned)
                    {
                        cell.SetState(GridCell.CellState.Empty);
                    } else
                    {
                        cell.SetState(GridCell.CellState.Blocked);
                    }
                }
            }
        }

        static int CalculateHeuristic(Node node1, Node node2)
        {
            return CalculateHeuristic(node1.x, node1.y, node2.x, node2.y);
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
