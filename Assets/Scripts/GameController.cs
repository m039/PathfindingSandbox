using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        GridView _GridView;

        #endregion

        GridCell _currentCell;

        GridCell _goalCell;

        static readonly RaycastHit2D[] s_Buffer = new RaycastHit2D[16];

        void Start()
        {
            Init();
        }

        void Init()
        {
            _GridView.Cells[0, 0].SetState(GridCell.CellState.StartNode);
        }

        void Update()
        {
            ProcessInput();
        }

        Coroutine _findPathCoroutine;

        public void OnFindPathClicked()
        {
            if (_goalCell == null)
            {
                Debug.LogWarning("Can't find a path, the goal node is missing!");
                return;
            }

            _findPathCoroutine = StartCoroutine(FindPathCoroutine());
        }

        IEnumerator FindPathCoroutine()
        {
            yield return new Pathfinder(_GridView).Search(_GridView.Cells[0, 0], _goalCell);
            _findPathCoroutine = null;
        }

        public void OnStopFindingPathClicked()
        {
            if (_findPathCoroutine != null)
            {
                StopCoroutine(_findPathCoroutine);
            }
        }

        void ProcessInput()
        {
            // The animation is running.
            if (_findPathCoroutine != null)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (_currentCell != null)
                {
                    var state = _currentCell.GetState();
                    if (state != GridCell.CellState.StartNode && state != GridCell.CellState.GoalNode)
                    {
                        if (_goalCell != null)
                        {
                            _goalCell.SetState(GridCell.CellState.Empty);
                        }
                        _goalCell = _currentCell;
                        _goalCell.SetState(GridCell.CellState.GoalNode);
                    } else if (state == GridCell.CellState.GoalNode)
                    {
                        if (_goalCell != null)
                        {
                            _goalCell.SetState(GridCell.CellState.Empty);
                        }
                        _goalCell = _currentCell;
                        _goalCell.SetState(GridCell.CellState.Empty);
                    }
                }
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var count = Physics2D.GetRayIntersectionNonAlloc(ray, s_Buffer);
            for (int i = 0; i < count; i++)
            {
                if (s_Buffer[i].collider.GetComponentInParent<GridCell>() is GridCell gridCell)
                {
                    if (_currentCell != null && _currentCell != gridCell)
                    {
                        _currentCell.SetHighlighted(false);
                        _currentCell = null;
                    }

                    if (_currentCell == null)
                    {
                        _currentCell = gridCell;
                        _currentCell.SetHighlighted(true);
                    }

                    return;
                }
            }

            if (_currentCell != null)
            {
                _currentCell.SetHighlighted(false);
                _currentCell = null;
            }
        }
    }
}
