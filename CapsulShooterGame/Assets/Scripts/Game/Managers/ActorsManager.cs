using System.Collections.Generic;
using UnityEngine;

public class ActorsManager : MonoBehaviour
{
    [SerializeField] public List<Actor> Actors { get; private set; }
    [SerializeField] public GameObject Player { get; private set; }

    public void SetPlayer(GameObject player) => Player = player;

    void Awake()
    {
        Actors = new List<Actor>();
    }
}