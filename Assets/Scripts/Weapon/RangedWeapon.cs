using UnityEngine;

public abstract class RangedWeapon : Weapon
{
    [SerializeField] protected Ammunition ammunitionPrefab;
    [SerializeField] protected int quantityAmmo;

    [SerializeField] protected Transform attackPointTransform;
    [SerializeField] protected Transform ammoContainer;

    protected Pool<Ammunition> poolAmmo;
    protected Vector2 AttackPointPosition => attackPointTransform.position;

    public override bool Attack(Vector2 targetPointPosition)
    {
        if (onFireDelay)  
            return false;

        if (quantityAmmo <= 0) 
            return false;

        if (isPlayerWeapon)
            UpdateUI();

        quantityAmmo -= 1;
        poolAmmo.GetFreeElement(out Ammunition ammunition);
        Vector2 directionVector = (targetPointPosition - AttackPointPosition).normalized;
        ammunition.SetDirectionAndStart(directionVector, attackPointTransform.position);
        StartCoroutine(DelayAfterAttack());

        return true;
    }
}
