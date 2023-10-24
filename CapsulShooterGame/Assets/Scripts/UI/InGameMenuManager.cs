using UnityEngine;
using UnityEngine.UI;

public class InGameMenuManager : MenuManager
{
    [SerializeField] private Button _buttonBack;

    protected override void Awake()
    {
        base.Awake();

        _buttonBack.onClick.AddListener(SetActiveMenu);
    }

    protected override void Exit() => _sceneLoader.LoadingScene();
    
    public void SetActiveMenu() => this.gameObject.SetActive(!IsActive);
}
