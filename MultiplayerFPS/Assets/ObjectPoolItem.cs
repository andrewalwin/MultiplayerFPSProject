using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    public int poolAmount;
    public GameObject objectToPool;
    public bool canExpand;
    public string objectTag;

    public ObjectPoolItem(int amount, GameObject obj, bool expand, string tag)
    {
        this.poolAmount = amount;
        this.objectToPool = obj;
        this.canExpand = expand;
        this.objectTag = tag;
    }
}

