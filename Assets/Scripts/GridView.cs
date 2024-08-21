using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridView : MonoBehaviour
    {
        #region Inspector

        public float width = 10;

        public float height = 10;

        public int rows = 10;

        public int columns = 10;

        [SerializeField] float _CellBorderPercent = 0.1f;

        [SerializeField] GridCell _GridCellPrefab;

        #endregion

        public Node[,] Nodes { get; private set; }

        float GetCellWidth() => width / columns;

        float GetCellHeight() => height / rows;

        Transform _parent;

        Vector3 GetCellPosition(int x, int y)
        {
            var cellWidth = GetCellWidth();
            var cellHeight = GetCellHeight();
            var p = transform.position;
            var leftTopCorner = new Vector2(p.x - width / 2 + cellWidth / 2, p.y - height / 2 + cellHeight / 2);
            return new Vector3(leftTopCorner.x + x * cellWidth, leftTopCorner.y + y * cellHeight, p.z);
        }

        Vector3 GetCellSize()
        {
            var cellWidth = GetCellWidth();
            var cellHeight = GetCellHeight();
            return new Vector3((1 - _CellBorderPercent) * cellWidth, (1 - _CellBorderPercent) * cellHeight, 0);
        }

        public void CreateGrid(bool[,] grids)
        {
            if (_parent != null)
            {
                Destroy(_parent.gameObject);
            }

            columns = grids.GetLength(0);
            rows = grids.GetLength(1);

            _parent = new GameObject("<Cells>").transform;
            _parent.SetParent(transform);

            var cellSize = GetCellSize();

            Nodes = new Node[columns, rows];

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    var gridCell = Instantiate(_GridCellPrefab);
                    gridCell.transform.SetParent(_parent, false);

                    gridCell.transform.localScale = cellSize;
                    gridCell.transform.position = GetCellPosition(x, y);
                    
                    gridCell.SetTextVisibility(false);
                    gridCell.node = new Node(x, y, gridCell);
                    gridCell.name = $"GridCell({x},{y})";

                    Nodes[x, y] = gridCell.node;

                    if (grids[x, y])
                    {
                        gridCell.node.state = NodeState.Blocked;
                        gridCell.SetState(GridCell.CellState.Blocked);
                    } else
                    {
                        gridCell.node.state = NodeState.Open;
                        gridCell.SetState(GridCell.CellState.Empty);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));
        }
    }
}
