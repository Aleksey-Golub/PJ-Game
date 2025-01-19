using UnityEngine;

namespace Code.UI
{
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private LocalizationText _bodyLocText;

        public bool IsOpened => gameObject.activeInHierarchy;

        internal void Construct(string localizationKey)
        {
            _bodyLocText.SetKey(localizationKey);
        }

        internal void Open()
        {
            Logger.Log($"[UIPopup] {gameObject.name} opening");

            gameObject.SetActive(true);
        }

        internal void Close()
        {
            Logger.Log($"[UIPopup] {gameObject.name} closing");

            gameObject.SetActive(false);
        }
    }
}