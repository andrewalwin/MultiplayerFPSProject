using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour{

    public static ObjectPooler SharedInstance;

	public List<GameObject> pooledObjects;
	public List<ObjectPoolItem> itemsToPool;
		
	void Awake(){
		SharedInstance = this;
	}

	void Start(){
		pooledObjects = new List<GameObject>();
		PopulatePools();
	}

	//for creating an object pool outside of the inspector
	public void PopulatePools(){
		foreach(ObjectPoolItem item in itemsToPool){
			for(int i = 0; i < item.poolAmount; i++){
				GameObject pooledObj = (GameObject)Instantiate(item.objectToPool);
                pooledObj.tag = item.objectTag;
				pooledObj.SetActive(false);
				pooledObjects.Add(pooledObj);
			}
		}
	}

	//get our object by tag
	public GameObject GetPooledObject(string tag){
		for(int i = 0; i < pooledObjects.Count; i++){
			if(!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag){
				return pooledObjects[i];
			}
		}
		//no objects left but need one
		foreach (ObjectPoolItem item in itemsToPool){
		if(item.objectToPool.tag == tag){
		if(item.canExpand){
			GameObject newPooledObj = (GameObject)Instantiate(item.objectToPool);
			newPooledObj.SetActive(false);
			pooledObjects.Add(newPooledObj);
			return newPooledObj;
			}
		}
		}
        return null;
	}

    public void AddPooledItem(GameObject newPoolItem)
    {

    }
}
