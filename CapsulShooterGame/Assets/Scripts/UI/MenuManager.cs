using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuManager : MonoBehaviour
{
    [SerializeField] protected SceneLoader _sceneLoader;
    [SerializeField] protected Button _buttonExit;

    public bool IsActive => this.gameObject.activeSelf;

    protected virtual void Awake()
    {
        _buttonExit.onClick.AddListener(Exit);
    }

    protected virtual void Exit() => Application.Quit();
}
