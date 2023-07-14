using UnityEngine;

public class Bow : RangedWeapon
{
    public override void TakeUp(Transform weaponContainer)
    {
        base.TakeUp(weaponContainer);
        poolAmmo = new Pool<Ammunition>(ammunitionPrefab, ammoContainer, false);
        poolAmmo.CreatePool(1);
        poolAmmo.GetElemToIndex(0).GetComponent<Arrow>().LiftOnTheFloorEvent += Reload;
    }

    private void Reload()
    {
        quantityAmmo = 1;
        ReloadUI();
    }

    protected override void UpdateUI() => bullets[0].sprite = emptyBulletImage;
    private void ReloadUI() => bullets[0].sprite = fillBulletImage;
}
