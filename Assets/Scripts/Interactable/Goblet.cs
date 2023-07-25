using UnityEngine;
using UnityEngine.VFX;

public class Goblet : MonoBehaviour, IInteractable
{
    [SerializeField] private VisualEffect playEffect;
    [SerializeField] private ChoiceHandler choiceHandler;
    
    public InteractType Type { get; } = InteractType.Button;

    private Arena arenaScript;

    private void Awake()
    {
        arenaScript = FindObjectOfType<Arena>();
        arenaScript.OnArenaEnds += SpawnAgain;
    }
    
    public void Interact()
    {
        choiceHandler.SetMainText("Would you like to try your luck again?");
        choiceHandler.SetLeftButtonText("Yes");
        choiceHandler.AddActionToLeftButton(StartArena);

        choiceHandler.SetRightButtonText("No");
        choiceHandler.Appear();
    }

    private void StartArena()
    {
        arenaScript.SetupArena();
        arenaScript.SpawnNextWave();

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
