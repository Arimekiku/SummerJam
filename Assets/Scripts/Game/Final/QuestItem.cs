using UnityEngine;

public class QuestItem : MonoBehaviour, IInteractable, IDestroyed
{
    [SerializeField] private Stand standToOccupy;
    [SerializeField] private Dialogue questDialogue;
    [SerializeField] private ChoiceHandler choiceHandler;
    [SerializeField] private Sprite paintedSprite;
    [SerializeField] private Sprite erasedSprite;

    public InteractType Type { get; } = InteractType.Button;

    private void Awake()
    {
        choiceHandler = FindObjectOfType<ChoiceHandler>();
    }

    public void Interact()
    {
        choiceHandler.SetMainText("Time to decide fate of this world.");
        choiceHandler.SetLeftButtonText("Paint");
        choiceHandler.AddActionToLeftButton(() =>
        {
            standToOccupy.SetPositiveValue();
            standToOccupy.Occupy(paintedSprite);
            DestroyThisObject();
        });
        
        choiceHandler.SetRightButtonText("Erase");
        choiceHandler.AddActionToRightButton(() =>
        {
            standToOccupy.SetNegativeValue();
            standToOccupy.Occupy(erasedSprite);
            DestroyThisObject();
        });

        questDialogue.Interact();
    }

    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }
}