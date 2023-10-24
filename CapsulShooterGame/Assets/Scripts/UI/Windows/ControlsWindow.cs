using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControlsWindow : WindowCore
{
    [SerializeField] private Button _buttonBack;

    public bool IsActive => this.gameObject.activeSelf;

    private void Awake()
    {
        _buttonBack.onClick.AddListener(() => { Hide(); });
    }

    public void Init(UnityAction onBackButtonClicked)
    {
        _buttonBack.onClick.AddListener(onBackButtonClicked);
    }
}
