using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class InventoryResourceView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        internal void Init(Sprite sprite, int initialValue)
        {
            _image.sprite = sprite;
            Set(initialValue);
        }

        internal void Set(int newCount)
        {
            _text.text = newCount.ToString();

            if (newCount > 0 && !gameObject.activeSelf)
                gameObject.SetActive(true);
            else if (newCount == 0 && gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }
}