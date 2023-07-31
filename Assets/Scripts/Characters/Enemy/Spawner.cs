using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies;
    
    public Enemy Spawned { get; private set; }

    private void Awake()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        int rand = Random.Range(0, enemies.Count);
        Spawned = Instantiate(enemies[rand], transform);
    }
}
