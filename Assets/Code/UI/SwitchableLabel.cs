using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class SwitchableLabel : MonoBehaviour
    {
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private TextMeshProUGUI _label;

        internal event Action LeftClicked;
        internal event Action RightClicked;

        internal void Init(string labelText)
        {
            _label.text = labelText;
        }

        internal void SubscribeUpdates()
        {
            _leftButton.onClick.AddListener(OnLeftClick);
            _rightButton.onClick.AddListener(OnRightClick);
        }

        internal void Cleanup()
        {
            _leftButton.onClick.RemoveListener(OnLeftClick);
            _rightButton.onClick.RemoveListener(OnRightClick);
        }

        private void OnLeftClick()
        {
            LeftClicked?.Invoke();
        }

        private void OnRightClick()
        {
            RightClicked?.Invoke();
        }
    }
}