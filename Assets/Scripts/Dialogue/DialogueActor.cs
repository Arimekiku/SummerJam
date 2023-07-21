using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DialogueActor
{
    [SerializeField] private Sprite actorAvatar;
    [SerializeField] private string actorName;
    [SerializeField] private Image actorBox;

    public string Name => actorName;

    public void ApplyActorSprite()
    {
        actorBox.sprite = actorAvatar;
        actorBox.enabled = true;
    }

    public void RevertActorSprite()
    {
        actorBox.enabled = false;
        actorBox.sprite = null;
    }
}
