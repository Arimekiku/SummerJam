using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies;

    private void Start()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        int rand = Random.Range(0, enemies.Count);
        Instantiate(enemies[rand], transform);
    }
}
