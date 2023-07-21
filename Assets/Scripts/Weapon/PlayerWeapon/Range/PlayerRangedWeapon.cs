using System;
using UnityEngine;


public abstract class PlayerRangedWeapon : PlayerWeapon
{
    [SerializeField] protected Ammunition ammunitionPrefab;
    [SerializeField] protected float reloadDuration;
    [SerializeField] protected int maxNumberOfAmmo;
    
    [SerializeField] protected int currentAmmo;

    [SerializeField] protected Transform attackPointTransform;
    [SerializeField] protected Transform ammoContainer;

    protected Pool<Ammunition> poolAmmo;
    protected Vector2 AttackPointPosition => attackPointTransform.position;

    public Type AmmoType => ammunitionPrefab.GetType();

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
        poolAmmo.GetFreeElement(out Ammunition ammunition);
        Vector2 directionVector = (targetPointPosition - AttackPointPosition).normalized;
        ammunition.SetDirectionAndStart(directionVector, attackPointTransform.position);
        StartCoroutine(ReloadTimer());
    }

    public override void ThrowWeapon(Vector2 targetPointPosition)
    {
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
            return ammoInStock;
        }

        currentAmmo = maxNumberOfAmmo;
        return needToMaxAmmo;
    }
}
