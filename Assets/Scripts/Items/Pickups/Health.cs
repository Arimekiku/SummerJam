using UnityEngine;

public class Health : ItemBehaviour
{
    [SerializeField] private int healthToRestore;

    public override void Initialize(Player player)
    {
        print("Grabbed health");
    }
}
