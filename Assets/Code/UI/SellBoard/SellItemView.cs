using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    internal class SellItemView : MonoBehaviour
    {
        [SerializeField] private Image _sellItemImage;
        [SerializeField] private TextMeshProUGUI _sellItemCountText;
        [SerializeField] private TextMeshProUGUI _cointCountText;
        [SerializeField] private TextMeshProUGUI _sellText;
        [SerializeField] private Button _sellButton;
        [SerializeField] private AudioClip _sellButtonClickedClip;

        private ResourceType _resourceType;

        internal event Action<ResourceType> SellButtonClicked;

        internal void Construct()
        {
            _sellButton.onClick.AddListener(OnSellButtonClicked);
        }

        private void OnDestroy()
        {
            _sellButton.onClick.RemoveListener(OnSellButtonClicked);
        }

        internal void Init(Sprite sellItemSprite, int sellItemCount, int totalCost, ResourceType resourceType)
        {
            _resourceType = resourceType;
            _sellItemImage.sprite = sellItemSprite;
            SetData(sellItemCount, totalCost);
        }

        internal void SetData(int sellItemCount, int totalCost)
        {
            _sellItemCountText.text = sellItemCount.ToString();
            _cointCountText.text = totalCost.ToString();

            gameObject.SetActive(sellItemCount != 0);
        }

        private void OnSellButtonClicked()
        {
            AudioSource.PlayClipAtPoint(_sellButtonClickedClip, Camera.main.transform.position);
            SellButtonClicked?.Invoke(_resourceType);
        }
    }
}