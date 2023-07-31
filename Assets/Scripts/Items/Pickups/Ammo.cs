using UnityEngine;

public class Ammo : ItemBehaviour
{
    [SerializeField] private PlayerWeapon refillType;
    
    private const int CountAmmo = 12;

    public override void Initialize(Player player)
    {
        //player.RestoreAmmo(CountAmmo, refillType);
        print("Grabbed Ammo");
    }
}
