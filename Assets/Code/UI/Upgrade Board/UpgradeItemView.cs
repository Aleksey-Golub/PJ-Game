using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    internal class UpgradeItemView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _upgradeItemImage;
        [SerializeField] private TextMeshProUGUI _upgradeText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _upgradeCostText;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private AudioClip _upgradeButtonClickedClip;

        private ToolType _toolType;

        internal event Action<ToolType> UpgradeButtonClicked;

        internal void Construct()
        {
            _upgradeButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            _upgradeButton.onClick.RemoveListener(OnButtonClicked);
        }

        internal void Init(Sprite sprite, string upgradeText, string levelText, string upgradeCostText, ToolType type)
        {
            _toolType = type;
            _upgradeItemImage.sprite = sprite;
            SetData(upgradeText, levelText, upgradeCostText);
        }

        internal void SetData(string upgradeText, string levelText, string upgradeCostText)
        {
            _upgradeText.text = upgradeText;
            _levelText.text = levelText;
            _upgradeCostText.text = upgradeCostText;
        }

        private void OnButtonClicked()
        {
            AudioSource.PlayClipAtPoint(_upgradeButtonClickedClip, Camera.main.transform.position);
            UpgradeButtonClicked?.Invoke(_toolType);
        }
    }
}