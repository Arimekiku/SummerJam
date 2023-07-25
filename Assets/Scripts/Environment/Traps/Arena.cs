using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Arena : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemyPrefabs;
    [SerializeField] private List<int> wavesValues;
    [SerializeField] private VisualEffect spawnVFX;
    [SerializeField] private QuestItem itemToSpawn;
    [SerializeField] private ArenaDoor door;
    [SerializeField] private Player player;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private UnityEvent afterArena;
    [SerializeField] private UnityEvent beforeArena;

    private Collider2D coll;
    private readonly List<Enemy> enemiesInArena = new List<Enemy>();
    private List<int> copiedValues;
    private bool completed;

    public event Action OnArenaEnds;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        player.OnDeath += OnPlayerDeath;
    }

    private void OnEnemyDeath()
    {
        Enemy enemyToRemove = enemiesInArena.First(e => e.Health <= 0);
        enemiesInArena.Remove(enemyToRemove);
        
        if (enemiesInArena.Count == 0 && copiedValues.Count != 0)
            Invoke(nameof(SpawnNextWave), 2f);
        else if (enemiesInArena.Count == 0 && copiedValues.Count == 0)
            Invoke(nameof(EndArena), 2f);
    }

    private void OnPlayerDeath()
    {
        foreach (Enemy enemy in enemiesInArena)
            Destroy(enemy.gameObject);

        enemiesInArena.Clear();

        if (completed)
        {
            OnArenaEnds.Invoke();
            afterArena.Invoke();
        }
        else
        {
            trigger.gameObject.SetActive(true);
            door.Open();
        }
    }

    public void SetupArena()
    {
        copiedValues = new List<int>(wavesValues);
        door.Close();
        beforeArena.Invoke();
    }

    public void SpawnNextWave()
    {
        int storedValue = copiedValues[0];
        copiedValues.Remove(storedValue);

        for (int i = 0; i < storedValue; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], GetValidSpawnPosition(), quaternion.identity);
            Instantiate(spawnVFX, newEnemy.transform.position, quaternion.identity);
            newEnemy.OnDeath += OnEnemyDeath;
            
            enemiesInArena.Add(newEnemy);
        }
        AudioHandler.PlaySound(spawnSound);

        Vector2 GetValidSpawnPosition()
        {
            Bounds collBounds = coll.bounds;
            
            Vector2 spawnPosition = Vector2.zero;
            bool canSpawn = false;

            while (!canSpawn)
            {
                spawnPosition = RandomColliderPoint();
                Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 1f);

                foreach (Collider2D overlapColl in colliders)
                {
                    if (overlapColl.TryGetComponent(out Character _)) 
                        continue;
                    
                    if (!overlapColl.isTrigger)
                        continue;

                    canSpawn = true;
                }
            }

            return spawnPosition;

            Vector2 RandomColliderPoint()
            {
                float randomX = Random.Range(collBounds.min.x, collBounds.max.x);
                float randomY = Random.Range(collBounds.min.y, collBounds.max.y);

                return new Vector2(randomX, randomY);
            }
        }
    }

    private void EndArena()
    {
        if (itemToSpawn)
        {
            itemToSpawn.gameObject.SetActive(true);
            Instantiate(spawnVFX, itemToSpawn.transform.position, quaternion.identity);
            completed = true;
        }

        door.Open();
        OnArenaEnds.Invoke();
    }
}
