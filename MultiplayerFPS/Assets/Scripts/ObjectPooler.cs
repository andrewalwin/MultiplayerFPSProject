using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public string name;
        public int size;
    }

    public List<Pool> pools = new List<Pool>();
    public Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    public static ObjectPooler instance;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }

    void Start() {
        //spawn any objects that have been added to our pool list not at runtime
        //PopulatePools();
    }

    public GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(name))
        {
            Debug.Log("Pool with name: " + name + " does not exist");
            return null;
        }
        Queue<GameObject> objectToSpawnQueue = poolDictionary[name];
        if (objectToSpawnQueue.Count > 0)
        {
            //if we've found a valid object, spawn it
            if (objectToSpawnQueue.Peek() != null)
            {
                GameObject spawnObject = objectToSpawnQueue.Dequeue();
                spawnObject.SetActive(true);
                spawnObject.transform.position = position;
                spawnObject.transform.rotation = rotation;
                //reset the objects velocity in case it had one before being set to false
                if (spawnObject.GetComponent<Rigidbody>() != null)
                {
                    spawnObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                return spawnObject;
            }
        }
        //didnt find an inactive object, need to add a new one to the pool (could also just spawn a NEW one instead of increasing our pool size which may be better
        ExpandPool(name, 1);
        return SpawnFromPool(name, position, rotation);
    }

    //expand the total of our item pool
    public void ExpandPool(string name, int size)
    {
        foreach (Pool pool in pools)
        {
            if (pool.name.Equals(name))
            {
                GameObject objPrefab = pool.prefab;
                    Queue<GameObject> existingPool = poolDictionary[name];
                    {
                    for (int i = 0; i <= size; i++)
                    {
                        GameObject obj = Instantiate(objPrefab);
                        obj.SetActive(false);
                        existingPool.Enqueue(obj);
                    }
                    return;
                }
            }
        }
    }

        //Create a new pool of objects/expand an existing pool
        public void AddPool(GameObject prefab, string name, int size)
        {
        //if pool of this type already exists, just expand upon it
        if (poolDictionary.ContainsKey(name))
        {
            ExpandPool(name, size);
            return;
        }
       
            //create the new pool we are going to add
            Pool newPool = new Pool();
            newPool.prefab = prefab;
            newPool.name = name;
            newPool.size = size;
            pools.Add(newPool);

            Queue<GameObject> newPoolQueue = new Queue<GameObject>();
            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
            newPoolQueue.Enqueue(obj);
            }
            poolDictionary.Add(name, newPoolQueue);

            //pools.Add(newPool);

            ////populate our pools
            //PopulatePools();
        }

        //re-pool an object
        public void RePool(GameObject obj, string name)
        {
            if (poolDictionary.ContainsKey(name))
            {
            obj.transform.parent = null;
                poolDictionary[name].Enqueue(obj);
            }
        }
    }


