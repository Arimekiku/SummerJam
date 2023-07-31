using UnityEngine;

[CreateAssetMenu(fileName = "Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private ItemBehaviour context;

    public Sprite ItemSprite => itemSprite;
    public ItemBehaviour Behaviour => context;
}