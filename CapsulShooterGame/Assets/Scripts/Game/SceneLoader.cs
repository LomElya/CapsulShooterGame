using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private int IDNextScene;
    [SerializeField] private GameObject _progressBar;
    [SerializeField] private Image _progressBarImage;
    [SerializeField] private TMP_Text _loadingText;

    private AsyncOperation _asyncOperation;

    public void LoadingScene()
    {
        EventManager.StopGame();
        StartCoroutine("LoadScene");
    }

    private IEnumerator LoadScene()
    {
        float loadingProgress;
        _asyncOperation = SceneManager.LoadSceneAsync(IDNextScene);
        _progressBar.SetActive(true);

        while (!_asyncOperation.isDone)
        {
            loadingProgress = Mathf.Clamp01(_asyncOperation.progress / 0.9f);
            _loadingText.text = $"Loading... {(loadingProgress * 100).ToString("0")}%";
            _progressBarImage.fillAmount = loadingProgress;
            yield return null;
        }
    }

}
