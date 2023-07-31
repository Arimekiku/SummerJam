using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private List<ItemData> possibleLoot;
    
    public event Action<ItemData, Vector2> OnInteract;
    
    public void Interact()
    {
        ItemData randomItem = possibleLoot[Random.Range(0, possibleLoot.Count)];
        
        OnInteract.Invoke(randomItem, transform.position);
        Destroy(gameObject);
    }
}