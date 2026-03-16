using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T m_Prefab;
    private readonly List<T> m_Pool = new List<T>();
    private readonly Transform m_Parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        m_Prefab = prefab;
        m_Parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(m_Prefab, m_Parent);
            obj.gameObject.SetActive(false);
            m_Pool.Add(obj);
        }
    }

    public T Get()
    {
        foreach (var obj in m_Pool)
        {
            if (!obj.gameObject.activeSelf)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        // pool exhausted, create a new one
        T newObj = Object.Instantiate(m_Prefab, m_Parent);
        m_Pool.Add(newObj);
        return newObj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    public void ReturnAll()
    {
        foreach (var obj in m_Pool)
        {
            if (obj != null)
                obj.gameObject.SetActive(false);
        }
    }
}
