using System;
using UnityEngine;

public class Item : PoolableObject, IInteractable
{
    private ItemData context;
    private SpriteRenderer rend;

    public event Action<ItemBehaviour> OnInteract;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void Initialize(ItemData newContext)
    {
        rend.sprite = newContext.ItemSprite;
        context = newContext;
    }

    public void Interact()
    {
        OnInteract.Invoke(context.Behaviour);
        gameObject.SetActive(false);
    }
}