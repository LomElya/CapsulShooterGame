using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomPool<T> where T : MonoBehaviour
{
    private T _prefab;
    private List<T> _objects;

    private void OnEnable()
    {
        EventManager.OnStartGame.AddListener(StartGame);
    }

    public CustomPool(T prefab, int prewarmObjects, Transform parent = null)
    {

        _prefab = prefab;
        _objects = new List<T>();

        for (int i = 0; i < prewarmObjects; i++)
        {
            T obj;
            if (parent == null)
                obj = GameObject.Instantiate(_prefab);
            else
                obj = GameObject.Instantiate(_prefab, parent);

            obj.gameObject.SetActive(false);
            _objects.Add(obj);
        }
    }

    public T Get()
    {
        var obj = _objects.FirstOrDefault(x => !x.isActiveAndEnabled);

        if (obj == null)
        {
            obj = Create();
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private T Create()
    {
        var obj = GameObject.Instantiate(_prefab);
        _objects.Add(obj);
        return obj;
    }

    private void StartGame(LevelData levelData)
    {
        if (_objects == null)
            return;

        foreach (var obj in _objects)
        {
            GameObject.Destroy(obj.gameObject);
        }
        _objects.Clear();
    }
}
