using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelsConfig _levelsConfig;
    [SerializeField] private Transform _parentLevel;
    [SerializeField] private Level _startLevel;
    [SerializeField] private bool isHardMode = false;

    private int _maxLevel => _levelsConfig.Levels.Count;
    private int _currentLevelID;
    private LevelData _currentLevelData;

    public UnityAction<LevelData> OnLevelStart;
    public UnityAction OnStartHardMode;
    public UnityAction OnLevelPassed;

    public bool IsHardMode => isHardMode;

    private void OnEnable()
    {
        OnLevelStart += StartLevel;

        EventManager.OnNextLevel.AddListener(NextLevel);
        EventManager.OnStartHardMode.AddListener(OnHardMode);
    }
    private void Start()
    {
        EventManager.StopGame();

        _currentLevelID = 0;
        _currentLevelData = GetLevels().FirstOrDefault(x => x.ID == _currentLevelID);
        if (_currentLevelData == null)
        {
            Debug.LogErrorFormat("Уровень с ID {0} не найден", _currentLevelID);
            return;
        }

        OnLevelStart?.Invoke(_currentLevelData);
    }

    private void StartLevel(LevelData levelData)
    {
        if (_startLevel != null)
            Destroy(_startLevel.gameObject);

        Level newLevel = Instantiate(levelData.LevelPrefab, _parentLevel);

        if (isHardMode)
        {
            EventManager.StartHardMode(true);
            HardLevel hardLevel = Instantiate(levelData.HardLevelPrefab, newLevel.transform);
        }

        _startLevel = newLevel;
    }

    public void NextLevel()
    {
        if (_currentLevelID + 1 > _maxLevel - 1)
        {
            if (!isHardMode)
            {
                EventManager.StartHardMode(true);
                _currentLevelID = 0;
            }
            else
            {
                EventManager.WinGame();
                return;
            }
        }
        else
            _currentLevelID++;

        SelectLevel(_currentLevelID);
    }

    public void RestartLevel()
    {
        OnLevelStart?.Invoke(_currentLevelData);
    }

    public void RestartGame()
    {
        //isHardMode = false;
        _currentLevelID = 0;
        _currentLevelData = GetLevels().FirstOrDefault(x => x.ID == _currentLevelID);

        EventManager.StartHardMode(false);
        OnLevelStart?.Invoke(_currentLevelData);
    }

    private void SelectLevel(int level)
    {
        _currentLevelID = level;
        _currentLevelData = GetLevels().FirstOrDefault(x => x.ID == _currentLevelID);
        OnLevelStart?.Invoke(_currentLevelData);
    }


    private void OnHardMode(bool status)
    {
        isHardMode = status;
    }

    public IEnumerable<LevelData> GetLevels()
    {
        return _levelsConfig.Levels;
    }
}