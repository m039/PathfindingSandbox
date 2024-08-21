using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class GridCell : MonoBehaviour
    {
        public enum CellState
        {
            Empty, StartNode, GoalNode, Frontier, Opened, Blocked
        }

        #region Inspector

        [SerializeField]
        TMPro.TMP_Text _F;

        [SerializeField]
        TMPro.TMP_Text _H;

        [SerializeField]
        TMPro.TMP_Text _G;

        [SerializeField]
        SpriteRenderer _Renderer;

        [SerializeField]
        StateData[] _StateData;

        #endregion

        public float f { set => _F.text = value.ToString(); }

        public float g { set => _G.text = value.ToString(); }

        public float h { set => _H.text = value.ToString(); }

        public Node node { get; set; }

        public Color color { set => _Renderer.color = value; }

        CellState _state;

        bool _highlighted;

        public CellState GetState() => _state;

        public void SetTextVisibility(bool value)
        {
            _F.gameObject.SetActive(value);
            _G.gameObject.SetActive(value);
            _H.gameObject.SetActive(value);
        }

        public void SetHighlighted(bool value)
        {
            _highlighted = value;
            UpdateState();
        }

        public void SetState(CellState state)
        {
            _state = state;
            UpdateState();
        }

        void UpdateState()
        {
            foreach (var data in _StateData)
            {
                if (data.state == _state)
                {
                    if (_highlighted)
                    {
                        _Renderer.color = data.highlighted;
                    } else
                    {
                        _Renderer.color = data.normal;
                    }
                    break;
                }
            }
        }

        [System.Serializable]
        class StateData
        {
            public CellState state;
            public Color normal = Color.white;
            public Color highlighted = Color.gray;
        }
    }
}
