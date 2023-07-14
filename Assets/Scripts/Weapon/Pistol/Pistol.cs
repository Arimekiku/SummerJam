using UnityEngine;

public class Pistol : RangedWeapon
{
    public override void TakeUp(Transform weaponContainer)
    {
        base.TakeUp(weaponContainer);
        poolAmmo = new Pool<Ammunition>(ammunitionPrefab, ammoContainer, true);
        poolAmmo.CreatePool(4);
    }

    public override void CastOut(Vector2 PlayerPosition)
    {
        base.CastOut(PlayerPosition);
        poolAmmo.DestroyPool();
    }

    protected override void UpdateUI()
    {
        for (int i = bullets.Length - 1; i >= 0; i--) 
        {
            if (bullets[i].sprite == fillBulletImage) 
            {
                bullets[i].sprite = emptyBulletImage;
                return;
            }
        }
    }
}
