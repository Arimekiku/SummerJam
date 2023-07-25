using UnityEngine;

public class Stand : MonoBehaviour
{
    [SerializeField] private SpriteRenderer content;
    
    public int StandValue { get; private set; }
    public bool IsOccupied { get; private set; }

    public void Occupy(Sprite endGameItem)
    {
        content.sprite = endGameItem;
        IsOccupied = true;
    }

    public void SetPositiveValue()
    {
        StandValue = 1;
    }

    public void SetNegativeValue()
    {
        StandValue = -1;
    }
}
