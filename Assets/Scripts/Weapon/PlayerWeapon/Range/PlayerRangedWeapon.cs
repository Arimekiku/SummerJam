using System;
using UnityEngine;


public abstract class PlayerRangedWeapon : PlayerWeapon
{
    [SerializeField] protected Ammunition ammunitionPrefab;
    [SerializeField] protected AmmunitionUI ammunitionUIPrefab;
    [SerializeField] protected Sprite weaponUISprite;
    [SerializeField] protected float reloadDuration;
    [SerializeField] protected int maxNumberOfAmmo;
    
    [SerializeField] protected int currentAmmo;
    public AmmunitionUI[] currentAmmoUI;

    [SerializeField] protected Transform attackPointTransform;
    [SerializeField] protected Transform ammoContainer;

    protected Pool<Ammunition> poolAmmo;
    protected Vector2 AttackPointPosition => attackPointTransform.position;
    public int MaxAmmo => maxNumberOfAmmo;
    public int CurrentAmmo => currentAmmo;
    public AmmunitionUI AmmoUI => ammunitionUIPrefab;
    public Sprite WeaponSprite => weaponUISprite;

    private void Start()
    {
        currentAmmo = maxNumberOfAmmo;
    }

    public override void Attack(Vector2 targetPointPosition)
    {
        if (isReloading) 
            return;

        if (currentAmmo <= 0) 
            return;

        UpdateUI();

        currentAmmo -= 1;
        currentAmmoUI[currentAmmo].Deplete();
        
        poolAmmo.GetFreeElement(out Ammunition ammunition);
        Vector2 directionVector = (targetPointPosition - AttackPointPosition).normalized;
        ammunition.SetDirectionAndStart(directionVector, attackPointTransform.position);
        AudioHandler.PlaySound(releaseSound);
        StartCoroutine(ReloadTimer());
    }

    public override void ThrowWeapon(Vector2 targetPointPosition)
    {
        foreach (AmmunitionUI ammoUI in currentAmmoUI)
            Destroy(ammoUI.gameObject);
        
        poolAmmo.DestroyPool();
        base.ThrowWeapon(targetPointPosition);
    }

    protected override void TakeUp(Transform weaponContainer)
    {
        poolAmmo = new Pool<Ammunition>(ammunitionPrefab, ammoContainer, true);
        poolAmmo.CreatePool(1);
        
        foreach (Ammunition ammunition in poolAmmo.PoolList)
            ammunition.SetDamage(damage);

        base.TakeUp(weaponContainer);
    }

    public override void CastOut(Vector2 playerPosition)
    {
        foreach (AmmunitionUI ammoUI in currentAmmoUI)
            Destroy(ammoUI.gameObject);

        currentAmmoUI = null;

        poolAmmo.DestroyPool();
        base.CastOut(playerPosition);
    }

    public int Reload(int ammoInStock)
    {
        int needToMaxAmmo = maxNumberOfAmmo - currentAmmo;
        
        if (needToMaxAmmo == 0 || ammoInStock <= 0)
            return 0;
        
        if (ammoInStock <= needToMaxAmmo)
        {
            currentAmmo += ammoInStock;

            for (int i = 0; i < currentAmmo; i++)
                currentAmmoUI[i].Reload();
            
            return ammoInStock;
        }

        currentAmmo = maxNumberOfAmmo;
        
        for (int i = 0; i < currentAmmo; i++)
            currentAmmoUI[i].Reload();
        
        return needToMaxAmmo;
    }
}
