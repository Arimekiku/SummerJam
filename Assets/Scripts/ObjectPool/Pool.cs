using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pool<T> where T : PoolableObject
{
    private readonly T prefab;
    private readonly Transform container;

    private readonly bool autoExpand;

    public T[] Entities => poolList.ToArray();
    
    private List<T> poolList;

    public Pool(T prefab, Transform container = null, bool autoExpand = true)
    {
        this.prefab = prefab;
        this.container = container;
        this.autoExpand = autoExpand;

        if (this.container is null)
        {
            this.container = Object.Instantiate(new GameObject()).transform;
            this.container.name = $"Pool {typeof(T)}";
        }
    }

    public void CreatePool(int count)
    {
        poolList = new List<T>();

        for (int i = 0; i < count; i++)
            CreateObject();
    }

    public void DestroyPool()
    {
        foreach (T objectToDestroy in poolList)
            objectToDestroy.DestroyThisObject();
        
        poolList.Clear();
    }
    
    private T CreateObject(bool setActiveByDefault = false)
    {
        T newObject = Object.Instantiate(prefab, container);
        newObject.gameObject.SetActive(setActiveByDefault);
        poolList.Add(newObject);
        return newObject;
    }

    public void GetFreeElement(out T freeObjectInPool)
    {
        freeObjectInPool = poolList.First(o => !o.gameObject.activeSelf);

        if (freeObjectInPool)
        {
            freeObjectInPool.gameObject.SetActive(true);
            return;
        }

        if (!autoExpand) 
            throw new System.Exception($"The pool ran out of objects with the type {typeof(T)}");
        
        freeObjectInPool = CreateObject();
    }
}
