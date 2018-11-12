using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDisplaySpawnable : MonoBehaviour {

    //object has to have the same forward as the object you want to spawn, or else it'll spawn at weird angles
    [SerializeField]
    private GameObject spawnableObject;
    private GameObject spawnableIns;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float spawnRange;
    [SerializeField]
    private float spawnAngleMax;

    [SerializeField]
    private LayerMask ignoreCollisions;

    private Vector3 spawnableRotation;

    private bool isDisplaying = false;
	private bool canSpawn;

    [SerializeField]
    private KeyCode abilityKey;
    
    void Start()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
        //can do our creation of spawnableobject here if we don't use a prefab
        if (spawnableObject != null)
        {
            spawnableIns = GameObject.Instantiate(spawnableObject);
            spawnableIns.SetActive(false);
        }

        spawnableRotation = transform.forward;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Current displayable: " + isDisplaying);
            isDisplaying = !isDisplaying;
        }

        if (isDisplaying)
        {
            if (Input.GetMouseButton(1))
            {
                //spawnableRotation = (spawnableRotation + 2.0f) % 360;
            }
            TryDisplaySpawnable();
        }
        else
        {
            spawnableIns.SetActive(false);
            canSpawn = false;
        }
    }

    void TryDisplaySpawnable()
    {
        if (cam != null)
        {
            Vector3 rayOrigin = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, cam.transform.forward, out hit, spawnRange))
            {
                spawnableIns.transform.position = hit.point;
                spawnableIns.transform.forward = spawnableRotation;
                spawnableIns.transform.up = hit.normal;
                if (Vector3.Angle(hit.normal, Vector3.up) <= spawnAngleMax)
                {
                    //make color "valid" maybe so player knows they can spawn it
                    //maybe enable above so we're not CONSTANTLY enabling the object
                    spawnableIns.SetActive(true);
                    canSpawn = true;
                }
                else
                {
                    Debug.Log("Invalid");
                    //make color "invalid" maybe
                    spawnableIns.SetActive(false);
                    canSpawn = false;
                }
            }
            else
            {
                spawnableIns.SetActive(false);
                canSpawn = false;
            }
        }
    }
}
