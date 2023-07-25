using System.Collections;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;
    [Space, SerializeField] private Ammunition arrowPrefab;
    [SerializeField] private AudioClip releaseClip;

    private Transform currentPoint;

    public void BeginFire()
    {
        currentPoint = firstPoint;
        Attack();
    }

    public void EndFire()
    {
        StopAllCoroutines();
    }

    private void Attack()
    {
        currentPoint = currentPoint == firstPoint ? secondPoint : firstPoint;

        StartCoroutine(AttackCooldown());

        IEnumerator AttackCooldown()
        {
            float currentTime = 1f;

            while (currentTime > 0)
            {
                yield return new WaitForFixedUpdate();
                currentTime -= Time.fixedDeltaTime;
            }

            Ammunition currentArrow = Instantiate(arrowPrefab, transform);
            currentArrow.SetDirectionAndStart(transform.up, currentPoint.position);
            AudioHandler.PlaySound(releaseClip);
            Attack();
        }
    }
}
