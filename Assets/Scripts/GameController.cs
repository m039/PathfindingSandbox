using m039.Common.Pathfindig;
using System;
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

        [SerializeField]
        LineRenderer _LineRenderer;

        #endregion

        Node _currentNode;

        Node _goalNode;

        Node _startNode;

        static readonly RaycastHit2D[] s_Buffer = new RaycastHit2D[16];

        void Start()
        {
            Init();
        }

        void Init()
        {
            _startNode = _GridView.Nodes[0, 0];
            _startNode.cell.SetState(GridCell.CellState.StartNode);
        }

        void Update()
        {
            ProcessInput();
        }

        Coroutine _findPathCoroutine;

        public void OnFindPathClicked()
        {
            if (_goalNode == null)
            {
                Debug.LogWarning("Can't find a path, the goal node is missing!");
                return;
            }

            _findPathCoroutine = StartCoroutine(FindPathCoroutine());
        }

        IEnumerator FindPathCoroutine()
        {
            _LineRenderer.positionCount = 0;

            var pathfinder = new Pathfinder(this, _GridView);

            yield return pathfinder.Search(_startNode, _goalNode);

            _findPathCoroutine = null;
        }

        public void OnStopFindingPathClicked()
        {
            if (_findPathCoroutine != null)
            {
                StopCoroutine(_findPathCoroutine);
                _findPathCoroutine = null;
            }
        }

        void ProcessInput()
        {
            // The animation is running.
            if (_findPathCoroutine != null)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (_currentNode != null)
                {
                    if (_currentNode != _startNode && _currentNode != _goalNode)
                    {
                        if (_goalNode != null)
                        {
                            _goalNode.cell.SetState(GridCell.CellState.Empty);
                        }
                        _goalNode = _currentNode;
                        _goalNode.cell.SetState(GridCell.CellState.GoalNode);
                    } else if (_currentNode == _goalNode)
                    {
                        if (_goalNode != null)
                        {
                            _goalNode.cell.SetState(GridCell.CellState.Empty);
                        }
                        _goalNode = _currentNode;
                        _goalNode.cell.SetState(GridCell.CellState.Empty);
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (_currentNode != null)
                {
                    if (_currentNode != _startNode && _currentNode != _goalNode)
                    {
                        if (_currentNode.state == NodeState.Openned)
                        {
                            _currentNode.state = NodeState.Blocked;
                            _currentNode.cell.SetState(GridCell.CellState.Blocked);
                        } else
                        {
                            _currentNode.state = NodeState.Openned;
                            _currentNode.cell.SetState(GridCell.CellState.Empty);
                        }
                    }
                }
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var count = Physics2D.GetRayIntersectionNonAlloc(ray, s_Buffer);
            for (int i = 0; i < count; i++)
            {
                if (s_Buffer[i].collider.GetComponentInParent<GridCell>() is GridCell gridCell)
                {
                    if (_currentNode != null && _currentNode != gridCell.node)
                    {
                        _currentNode.cell.SetHighlighted(false);
                        _currentNode = null;
                    }

                    if (_currentNode == null)
                    {
                        _currentNode = gridCell.node;
                        _currentNode.cell.SetHighlighted(true);
                    }

                    return;
                }
            }

            if (_currentNode != null)
            {
                _currentNode.cell.SetHighlighted(false);
                _currentNode = null;
            }
        }

        public void DrawPath(List<Node> path)
        {
            if (path != null)
            {
                _LineRenderer.positionCount = path.Count;
                for (int i = 0; i < path.Count; i++)
                {
                    _LineRenderer.SetPosition(i, path[i].cell.transform.position);
                }
            } else
            {
                _LineRenderer.positionCount = 0;
            }
        }
    }
}
