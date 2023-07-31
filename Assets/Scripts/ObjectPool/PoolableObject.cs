using UnityEngine;

public abstract class PoolableObject : MonoBehaviour, IDestroyable
{
    public virtual void DestroyThisObject()
    {
        Destroy(gameObject);
    }
}