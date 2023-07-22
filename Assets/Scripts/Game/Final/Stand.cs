using UnityEngine;

public class Stand : MonoBehaviour
{
    [SerializeField] private SpriteRenderer content;

    public bool IsOccupied { get; private set; }

    public void Occupy(Sprite endGameItem)
    {
        content.sprite = endGameItem;
        IsOccupied = true;
    }
}
