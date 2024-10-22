using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    internal class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField, Range(0, 1)] private float _inactiveAlpha = 0.6f;
        [SerializeField] private Image _upgradeItemImage;
        [SerializeField] private TextMeshProUGUI _upgradeText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _upgradeCostText;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private AudioClip _upgradeButtonClickedClip;

        private string _itemID;
        private float _startAlpha;
        private bool _showButton = true;

        internal bool ShowButton => _showButton;
        internal event Action<string> UpgradeButtonClicked;

        internal void Construct()
        {
            _upgradeButton.onClick.AddListener(OnButtonClicked);

            _startAlpha = _canvasGroup.alpha;
        }

        private void OnDestroy()
        {
            _upgradeButton.onClick.RemoveListener(OnButtonClicked);
        }

        internal void Init(Sprite sprite, string itemID)
        {
            _itemID = itemID;
            _upgradeItemImage.sprite = sprite;
        }

        internal void SetData(string upgradeText, string levelText, string upgradeCostText, bool showButton)
        {
            _upgradeText.text = upgradeText;
            _levelText.text = levelText;
            _upgradeCostText.text = upgradeCostText;

            if (showButton != _showButton)
            {
                _showButton = showButton;
                _upgradeButton.gameObject.SetActive(_showButton);
                _canvasGroup.alpha = _showButton ? _startAlpha : _inactiveAlpha;
            }
        }

        private void OnButtonClicked()
        {
            AudioSource.PlayClipAtPoint(_upgradeButtonClickedClip, Camera.main.transform.position);
            UpgradeButtonClicked?.Invoke(_itemID);
        }
    }
}