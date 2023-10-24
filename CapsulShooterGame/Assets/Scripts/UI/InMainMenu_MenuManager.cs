using UnityEngine;
using UnityEngine.UI;

public class InMainMenu_MenuManager : MenuManager
{
    [SerializeField] private Button _buttonStart;
    [SerializeField] private Button _buttonOptions;

    [Header("Windows")]
    [SerializeField] private OptionsWindow _options;

    protected override void Awake()
    {
        base.Awake();

        _buttonStart.onClick.AddListener(StartLevel);
        _buttonOptions.onClick.AddListener(OpenOptions);
    }

    private void StartLevel() => _sceneLoader.LoadingScene();
    private void OpenOptions() => _options.SetActive();

}
