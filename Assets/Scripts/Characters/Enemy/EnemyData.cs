using UnityEngine;
using UnityEngine.VFX;

public abstract class EnemyData : ScriptableObject
{
    [SerializeField] private float detectionRange;
    [SerializeField] private float damageBoostSpeed;
    [SerializeField] private int maxHealth;
    [SerializeField] private Corpse corpsePrefab;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private VisualEffect deathVFX;
    
    public float DetectionRange => detectionRange;
    public float DamageBoostSpeed => damageBoostSpeed;
    public int MaxHealth => maxHealth;
    public Corpse CorpsePrefab => corpsePrefab;

    public void HandleDeath(Transform enemy)
    {
        PlayDeathClip();
        PlayDeathVFX(enemy);
        InstantiateDeathCorpse(enemy);
    }

    private void PlayDeathClip()
    {
        AudioHandler.PlaySound(deathClip);
    }

    private void PlayDeathVFX(Transform enemy)
    {
        VisualEffect effect = Instantiate(deathVFX, enemy.parent);
        effect.transform.position = enemy.position + Vector3.down * 0.5f;
        effect.transform.rotation = enemy.rotation;
    }

    private void InstantiateDeathCorpse(Transform enemy)
    {
        Corpse corpseToSpawn = Instantiate(CorpsePrefab, enemy.position, enemy.rotation);
        Vector2 randomVector = new Vector2(Random.value, Random.value);
        corpseToSpawn.Initialize(randomVector);
    }
}
