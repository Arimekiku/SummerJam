using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected float rateOfFirePerSec;
    [SerializeField] private Quaternion defaultRotation;

    [SerializeField] protected bool isPlayerWeapon;
    [SerializeField] private Transform firstContainer;
    [SerializeField] private Transform secondContainer;
    [SerializeField] protected Image[] bullets;
    [SerializeField] protected Sprite emptyBulletImage;
    [SerializeField] protected Sprite fillBulletImage;

    public bool OnTheFloor { get; protected set; } = true;

    protected bool onFireDelay;

    public abstract bool Attack(Vector2 target);
    protected abstract void UpdateUI();

    public virtual void CastOut(Vector2 PlayerPosition)
    {
        transform.SetParent(null, false);
        OnTheFloor = true;
        transform.position = PlayerPosition;
    }

    public virtual void TakeUp(Transform weaponContainer)
    {
        transform.SetParent(weaponContainer, false);
        OnTheFloor = false;
        transform.SetLocalPositionAndRotation(Vector2.zero, defaultRotation);
        secondContainer.gameObject.SetActive(false);
        firstContainer.gameObject.SetActive(true);
    }

    protected IEnumerator DelayAfterAttack()
    {
        onFireDelay = true;
        float time = 1 / rateOfFirePerSec;

        while (time > 0)
        {
            time -= Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        onFireDelay = false;
    }
}
