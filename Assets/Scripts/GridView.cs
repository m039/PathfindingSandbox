using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridView : MonoBehaviour
    {
        #region Inspector

        [SerializeField] int _Rows = 10;

        [SerializeField] int _Columns = 10;

        [SerializeField] float _Width = 10;

        [SerializeField] float _Height = 10;

        [SerializeField] float _CellBorderPercent = 0.1f;

        [SerializeField] GridCell _GridCellPrefab;

        #endregion

        public Node[,] Nodes { get; private set; }

        float GetCellWidth() => _Width / _Columns;

        float GetCellHeight() => _Height / _Rows;

        void Awake()
        {
            CreateGrid();
        }

        Vector3 GetCellPosition(int x, int y)
        {
            var cellWidth = GetCellWidth();
            var cellHeight = GetCellHeight();
            var p = transform.position;
            var leftTopCorner = new Vector2(p.x - _Width / 2 + cellWidth / 2, p.y - _Height / 2 + cellHeight / 2);
            return new Vector3(leftTopCorner.x + x * cellWidth, leftTopCorner.y + y * cellHeight, p.z);
        }

        Vector3 GetCellSize()
        {
            var cellWidth = GetCellWidth();
            var cellHeight = GetCellHeight();
            return new Vector3((1 - _CellBorderPercent) * cellWidth, (1 - _CellBorderPercent) * cellHeight, 0);
        }

        void CreateGrid()
        {
            var cellSize = GetCellSize();

            Nodes = new Node[_Columns, _Rows];

            for (int x = 0; x < _Columns; x++)
            {
                for (int y = 0; y < _Rows; y++)
                {
                    var gridCell = Instantiate(_GridCellPrefab);
                    gridCell.transform.SetParent(transform, false);

                    gridCell.transform.localScale = cellSize;
                    gridCell.transform.position = GetCellPosition(x, y);
                    gridCell.SetState(GridCell.CellState.Empty);
                    gridCell.SetTextVisibility(false);
                    gridCell.node = new Node(x, y, gridCell);
                    gridCell.name = $"GridCell({x},{y})";

                    Nodes[x, y] = gridCell.node;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, new Vector3(_Width, _Height, 0));
        }
    }
}
