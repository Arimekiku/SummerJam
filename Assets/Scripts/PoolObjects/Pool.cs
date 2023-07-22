using System.Collections.Generic;
using UnityEngine;


public class Pool<T> where T : MonoBehaviour
{
    private readonly T prefab;

    private readonly Transform container;

    private readonly bool autoExpand;
    
    public List<T> PoolList { get; private set; }

    public Pool(T prefab, Transform container, bool autoExpand)
    {
        this.prefab = prefab;
        this.container = container;
        this.autoExpand = autoExpand;
    }

    public void CreatePool(int count)
    {
        PoolList = new List<T>();

        for (int i = 0; i < count; i++)
        {
            CreateObject();
        }
    }

    public void DestroyPool()
    {
        foreach (T objectToDestroy in PoolList)
        {
            IDestroyed destroyed = objectToDestroy.GetComponent<IDestroyed>();
            destroyed?.DestroyThisObject();
        }
        
        PoolList.Clear();
    }


    private T CreateObject(bool setActiveByDefault = false)
    {
        T newObject = Object.Instantiate(prefab, container);
        newObject.gameObject.SetActive(setActiveByDefault);
        PoolList.Add(newObject);
        return newObject;
    }

    public bool GetFreeElement(out T freeObjectInPool)
    {
        foreach (T objectInPool in PoolList)
        {
            if (!objectInPool.gameObject.activeInHierarchy)
            {
                freeObjectInPool = objectInPool;
                freeObjectInPool.gameObject.SetActive(true);
                return true;
            }
        }

        AutoExpand(out T createdObject);

        freeObjectInPool = createdObject;
        return false;
    }

    private void AutoExpand(out T createdObject)
    {
        if (autoExpand)
        {
            createdObject = CreateObject();
            return;
        }

        createdObject = null;

        throw new System.Exception($"The pool ran out of objects with the type {typeof(T)}");
    }

    public T GetElementIndex(int index)
    {
        if (index < 0 || index > PoolList.Count - 1)
            throw new System.Exception("Out of range");
        
        return PoolList[index];
    }
}
