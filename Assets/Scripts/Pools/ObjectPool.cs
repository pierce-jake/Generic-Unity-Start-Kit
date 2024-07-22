using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{

    public T prefab;
    [Min(4)] [SerializeField] private int maxPoolSize = 32;
    private readonly Queue<T> pool = new();

    public void Give(params T[] objs)
    {
        foreach (T obj in objs)
        {
            if (!obj)
                return;

            if(pool.Count <= maxPoolSize)
                pool.Enqueue(obj);

            obj.gameObject.SetActive(false);
        }
    }

    public T Get()
    {
        if (pool.Count == 0)
        {
            T result = Instantiate(prefab);
            result.transform.SetParent(transform, false);
            return Instantiate(prefab);
        }

        T obj = pool.Dequeue();
        obj.gameObject.SetActive(true);
        obj.transform.SetParent(transform, false);
        return obj;
    }

    public T[] GetAmount(int amount)
    {
        T[] result = new T[amount];

        for (int i = 0; i < result.Length; i++)
        {
            Debug.Log(pool.Count);
            if(pool.Count <= 0)
            {
                result[i] = Instantiate(prefab);
                result[i].transform.SetParent(transform, false);
                continue;
            }

            result[i] = pool.Dequeue();
            result[i].gameObject.SetActive(true);
            result[i].transform.SetParent(transform, false);
        }

        return result;
    }

}
