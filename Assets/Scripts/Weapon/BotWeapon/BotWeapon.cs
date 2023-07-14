using UnityEngine;

public class BotWeapon : RangedWeapon
{
    public override void TakeUp(Transform weaponContainer)
    {
        poolAmmo = new Pool<Ammunition>(ammunitionPrefab, ammoContainer, true);
        poolAmmo.CreatePool(5);
        OnTheFloor = false;
    }

    public override void CastOut(Vector2 PlayerPosition)
    {
        poolAmmo.DestroyPool();
        Destroy(gameObject);
    }

    protected override void UpdateUI()
    {
        throw new System.NotImplementedException();
    }
}
