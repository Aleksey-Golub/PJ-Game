using UnityEngine;

public class PlatformDependentSwitcherer : MonoBehaviour
{
    [SerializeField] private GameObject[] _desktopOn;
    [SerializeField] private GameObject[] _mobileOn;

    private void Start()
    {
        bool isMobile = Application.isMobilePlatform;

        foreach (GameObject go in _mobileOn)
            go.SetActive(isMobile);

        foreach (GameObject go in _desktopOn)
            go.SetActive(!isMobile);
    }
}
