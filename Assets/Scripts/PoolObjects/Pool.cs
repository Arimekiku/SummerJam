using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pool<T> where T : MonoBehaviour
{
    public T prefab;

    public Transform container;

    public bool autoExpand;

    List<T> poolList;

    public Pool(T prefab, Transform container, bool autoExpand)
    {
        this.prefab = prefab;
        this.container = container;
        this.autoExpand = autoExpand;
    }

    public void CreatePool(int count)
    {
        poolList = new List<T>();

        for (int i = 0; i < count; i++)
        {
            CreateObject();
        }
    }

    public void DestroyPool()
    {
        IDestroyed destroyed;

        foreach (T objectByDestroy in poolList)
        {
            destroyed = objectByDestroy.GetComponent<IDestroyed>();
            if (destroyed != null)
            {
                destroyed.DestroyThisObject();
            }
        }
        poolList.Clear();
    }


    T CreateObject(bool setActiveByDefolt = false)
    {
        var newObject = Object.Instantiate(prefab, container);
        newObject.gameObject.SetActive(setActiveByDefolt);
        this.poolList.Add(newObject);
        return newObject;
    }

    public bool GetFreeElement(out T freeObjectInPool)
    {
        foreach (var objectInPool in poolList)
        {
            if (!objectInPool.gameObject.activeInHierarchy)
            {
                freeObjectInPool = objectInPool;
                freeObjectInPool.gameObject.SetActive(true);
                return true;
            }
        }

        AutoExpand(out var createdObject);

        freeObjectInPool = createdObject;
        return false;
    }

    void AutoExpand(out T createdObject)
    {
        if (autoExpand)
        {
            createdObject = CreateObject();
            return;
        }

        createdObject = null;

        throw new System.Exception($"The pool ran out of objects with the type {typeof(T)}");
    }

    public T GetElemToIndex(int index)
    {
        if (index < 0 || index > poolList.Count - 1)
            throw new System.Exception("Out of range");
        return poolList[index];
    }
}
