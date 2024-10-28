using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class ButtonSwitcher : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _on;
        [SerializeField] private Sprite _off;
        [SerializeField] private TextMeshProUGUI _label;

        internal event Action<ButtonSwitcher> OnClicked;

        internal void SubscribeUpdates()
        {
            _button.onClick.AddListener(OnClick);
        }

        internal void Set(bool isOn, string labelText)
        {
            _image.sprite = isOn ? _on : _off;
            _label.text = labelText;
        }

        internal void Cleanup()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick() => OnClicked?.Invoke(this);
    }
}