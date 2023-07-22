using UnityEngine;
using UnityEngine.VFX;

public class Goblet : MonoBehaviour, IInteractable
{
    [SerializeField] private VisualEffect playEffect;
    
    public InteractType Type { get; } = InteractType.Button;

    private Arena arenaScript;

    private void Awake()
    {
        arenaScript = FindObjectOfType<Arena>();
        arenaScript.OnArenaEnds += SpawnAgain;
    }
    
    public void Interact()
    {
        arenaScript.SetupArena();
        arenaScript.SpawnNextWave();
        arenaScript.Door.Close();

        Instantiate(playEffect, transform.parent);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void SpawnAgain()
    {
        Instantiate(playEffect, transform.parent);
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }
}
