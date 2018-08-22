using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour {

    ObjectPooler objectPooler;

    private void Start()
    {
        objectPooler = ObjectPooler.instance;
    }

    //when the object is disabled, re-pool it
    public void RePoolObject()
    {
        objectPooler.RePool(gameObject, gameObject.name.Replace("(Clone)", ""));
    }

}
