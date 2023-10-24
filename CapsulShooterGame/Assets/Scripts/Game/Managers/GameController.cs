using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    [SerializeField] private AssetActor _asset;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private LevelController _levelController;
    private bool isHardMode = false;
    private List<Death> _deathPlayer;

    private Perosnage _playerPrefab => _asset.PersonagePrefab;
    private Death _playerDeath => _asset.DeathPrefab;

    private void OnEnable()
    {
        _levelController.OnLevelStart += StartLevel;

        EventManager.OnPlayerDeath.AddListener(DeathPlayer);
        EventManager.OnNextLevel.AddListener(NextLevel);
        EventManager.OnStartHardMode.AddListener(HardMode);
        EventManager.OnWinGame.AddListener(WinGame);
    }

    private void Awake()
    {
        _deathPlayer = new List<Death>();
    }

    private void Start()
    {
        AlertWindow alert = WindowsManagers.ShowDialog<AlertWindow>();

        alert.Init(StringConstant.ENG_AlertStartGeme, () =>
        {
            EventManager.ContinueGame();
            Debug.Log("Игра началась");
        });
    }

    private void StartLevel(LevelData levelData)
    {
        EventManager.StartGame(levelData);
    }

    private void NextLevel()
    {
        if (_deathPlayer != null)
        {
            foreach (Death death in _deathPlayer)
            {
                Destroy(death.gameObject);
            }

            _deathPlayer.Clear();
        }
    }

    private void DeathPlayer(Vector3 position)
    {
        EventManager.StopGame();

        Death deathOnject = Instantiate(_playerDeath, position, Quaternion.identity);
        _deathPlayer.Add(deathOnject);

        StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(_spawnDelay);

        _levelController.RestartLevel();
        EventManager.ContinueGame();

        Perosnage newPlayer = Instantiate(_playerPrefab);

        FollowCamera mainCamera = Camera.main.GetComponentInParent<FollowCamera>();
        mainCamera.Follow(newPlayer.transform);
    }

    private void HardMode(bool status)
    {
        if (isHardMode == status)
            return;

        isHardMode = status;

        AlertWindow alert = WindowsManagers.ShowDialog<AlertWindow>();

        alert.Init(StringConstant.ENG_AlertLastDoor, () =>
        {
            Debug.Log("Вкдючен хард мод. На F - самоуничтожение");
        });
    }

    private void WinGame()
    {
        AlertWindow alert = WindowsManagers.ShowDialog<AlertWindow>();

        alert.Init(StringConstant.ENG_AlertWinGame, () =>
        {
            Debug.Log("Игра выиграна");
            _levelController.RestartGame();
        });
    }
}
