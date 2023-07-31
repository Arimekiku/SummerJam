using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private Spawner[] spawners;
    
    private readonly List<Enemy> enemiesInRoom = new List<Enemy>();

    private void Start()
    {
        foreach (Spawner spawner in spawners)
            enemiesInRoom.Add(spawner.Spawned);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _))
            enemiesInRoom.ForEach(e => e.Activate());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _))
            enemiesInRoom.ForEach(e => e.Deactivate());
    }
}
