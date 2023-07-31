using UnityEngine;

[System.Serializable]
public struct DialoguePhrase
{
    [SerializeField] private int actorID;
    [SerializeField] private string phraseMessage;

    public string Message => phraseMessage;
    public int ActorID => actorID;
}
