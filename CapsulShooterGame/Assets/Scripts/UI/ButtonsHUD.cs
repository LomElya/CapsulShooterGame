using UnityEngine;
using UnityEngine.UI;

public class ButtonsHUD : MonoBehaviour
{
    [SerializeField] private InGameMenuManager _menu;
    [SerializeField] private Button _buttonMenu;

    private void OnEnable()
    {
        _buttonMenu.onClick.AddListener(OpenMenu);
    }

    private void OpenMenu() => _menu.SetActiveMenu();
}
