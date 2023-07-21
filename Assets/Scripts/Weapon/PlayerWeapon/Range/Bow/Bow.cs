using UnityEngine;


public class Bow : PlayerRangedWeapon
{
    protected override void TakeUp(Transform weaponContainer)
    {
        base.TakeUp(weaponContainer);
        
        foreach (Ammunition ammunition in poolAmmo.PoolList)
            ammunition.GetComponent<Arrow>().LiftOnTheFloorEvent += Reload;
    }

    private void Reload()
    {
        currentAmmo = 1;
    }

    protected override void UpdateUI()
    {
        
    }
}