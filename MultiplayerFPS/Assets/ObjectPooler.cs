using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour{

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public string name;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, List<GameObject>> poolDictionary;

    public static ObjectPooler Instance;
		
	void Awake(){
		Instance = this;
        poolDictionary = new Dictionary<string, List<GameObject>>();
    }

    void Start(){
        //spawn any objects that have been added to our pool list not at runtime
        PopulatePools();
	}

    public GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(name))
        {
            Debug.Log("Pool with name: " + name + " does not exist");
            return null;
        }
        List<GameObject> objectToSpawnList = poolDictionary[name];
        for (int i = 0; i < objectToSpawnList.Count; i++)
        {
            //if we've found a valid object, spawn it
            if (!objectToSpawnList[i].activeInHierarchy)
            {
                objectToSpawnList[i].SetActive(true);
                objectToSpawnList[i].transform.position = position;
                objectToSpawnList[i].transform.rotation = rotation;
                //reset the objects velocity in case it had one before being set to false
                if (objectToSpawnList[i].GetComponent<Rigidbody>() != null)
                {
                    objectToSpawnList[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                return objectToSpawnList[i];
            }
        }
        //didnt find an inactive object, need to add a new one to the pool (could also just spawn a NEW one instead of increasing our pool size which may be better
        ExpandPool(name);
        return SpawnFromPool(name, position, rotation);
    }

    //expand the total of our item pool
    public void ExpandPool(string name)
    {
        foreach (Pool pool in pools)
        {
            if(pool.name == name)
            {
                pool.size++;
            }
            PopulatePools();
        }
    }

    //instantiate the objects we need defined by our pools list
    public void PopulatePools()
    {
        foreach (Pool pool in pools)
        {
            //if we dont have a list for this object in our poolDictionary
            if (!poolDictionary.ContainsKey(pool.name))
            {
                List<GameObject> objectPool = new List<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Add(obj);
                }
                poolDictionary.Add(pool.name, objectPool);
            }
            //already have a list of this type of object, need to expand it
            else if (poolDictionary.ContainsKey(pool.name))
            {
                for(int i = 0; i < pool.size - poolDictionary[pool.name].Count; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    poolDictionary[pool.name].Add(obj);
                }
            }
        }
    }

    public void AddPool(GameObject obj, string name, int size)
    {
        //if pool of this type already exists, just expand upon it
        foreach(Pool pool in pools)
        {
            if(pool.name == name)
            {
                pool.size += size;
                PopulatePools();
                return;
            }
        }
        //create the new pool we are going to add
        Pool newPool = new Pool();
        newPool.prefab = obj;
        newPool.name = name;
        newPool.size = size;

        pools.Add(newPool);

        //populate our pools
        PopulatePools();
    }

}
