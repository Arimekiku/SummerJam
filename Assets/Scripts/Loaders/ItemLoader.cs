using UnityEngine;

public class ItemLoader : MonoBehaviour
{
    [SerializeField] private Transform itemPoolContainer;

    private Player player;
    
    private Pool<Item> items;
    private Chest[] chests;

    public void Initialize(Player newPlayer)
    {
        Item itemPrefab = Resources.Load("Prefabs/Interactables/Items/Item", typeof(Item)) as Item;
        items = new Pool<Item>(itemPrefab, itemPoolContainer);
        items.CreatePool(10);

        chests = FindObjectsOfType<Chest>();
        foreach (Chest chest in chests)
            chest.OnInteract += CreateItem;

        player = newPlayer;
    }

    private void CreateItem(ItemData context, Vector2 position)
    {
        items.GetFreeElement(out Item freeItem);
        
        freeItem.Initialize(context);
        freeItem.transform.position = position;
        freeItem.OnInteract += CreateBehaviour;
    }

    private void CreateBehaviour(ItemBehaviour context)
    {
        ItemBehaviour contextToInstantiate = Instantiate(context, null);
        contextToInstantiate.Initialize(player);
    }
}