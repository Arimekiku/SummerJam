using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : PlayerMeleeWeapon
{
    public override void Attack(Vector2 target)
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateUI()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator AttackAnimation() => throw new System.NotImplementedException();
}
