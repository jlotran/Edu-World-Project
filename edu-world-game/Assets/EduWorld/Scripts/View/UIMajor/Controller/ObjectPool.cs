using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly T prefab;
    private readonly Transform parent;

    private readonly List<T> pooledItems = new List<T>();
    private int currentIndex = 0;

    public ObjectPool(T prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
    }

    public void InitializePool(int size)
    {
        // Dọn dẹp pool cũ nếu có
        foreach (var item in pooledItems)
        {
            if (item != null)
                GameObject.Destroy(item.gameObject);
        }

        pooledItems.Clear();
        currentIndex = 0;

        // Tạo mới
        for (int i = 0; i < size; i++)
        {
            T item = GameObject.Instantiate(prefab, parent);
            item.gameObject.SetActive(false);
            pooledItems.Add(item);
        }
    }

    public T Get()
    {
        T item;

        if (currentIndex < pooledItems.Count)
        {
            item = pooledItems[currentIndex];
        }
        else
        {
            item = GameObject.Instantiate(prefab, parent);
            pooledItems.Add(item);
        }

        currentIndex++;
        item.gameObject.SetActive(true);
        return item;
    }

    public void ReturnAll()
    {
        for (int i = 0; i < currentIndex; i++)
        {
            pooledItems[i].gameObject.SetActive(false);
        }

        currentIndex = 0;
    }
}
