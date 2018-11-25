using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RaycastDisplaySpawnable : MonoBehaviour {

    //object has to have the same forward as the object you want to spawn, or else it'll spawn at weird angles
    [SerializeField]
    private GameObject spawnablePrefab;
    //display only
    private GameObject spawnableDisplayIns = null;
    //actual gameobject
    private GameObject spawnableIns;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float spawnRange;
    [SerializeField]
    private float spawnAngleMax;
    private float bottomDistance = 0f;
    private float currentRotation = 0f;

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
        if (spawnablePrefab != null)
        {
            SetupDisplayable();
            //spawnableIns = GameObject.Instantiate(spawnablePrefab);
            //spawnableIns.SetActive(false);
        }

        spawnableRotation = transform.forward;
    }

    void Update()
    {
        if (spawnableDisplayIns != null)
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
                    currentRotation += 30f * Time.deltaTime;
                }
                TryDisplaySpawnable();
            }
            else
            {
                spawnableDisplayIns.SetActive(false);
                canSpawn = false;
            }
        }
    }

    private void SetupDisplayable()
    {
        spawnableDisplayIns = GameObject.Instantiate(spawnablePrefab, new Vector3(0, 0, 0), spawnablePrefab.transform.rotation);
        spawnableDisplayIns.transform.localScale = spawnablePrefab.transform.localScale;
        //Mesh spawnableMesh = spawnableDisplayIns.GetComponentInChildren<MeshFilter>().sharedMesh;
        spawnableDisplayIns.SetActive(false);

        Bounds displayBounds = spawnableDisplayIns.GetComponentInChildren<Renderer>().bounds;
        Renderer[] displayRenderers = spawnableDisplayIns.GetComponentsInChildren<Renderer>();
        foreach(Renderer rend in displayRenderers)
        {
            if(rend != spawnableDisplayIns.GetComponent<Renderer>())
            {
                displayBounds.Encapsulate(rend.bounds);
            }
        }

        //bottomDistance = Vector3.Distance(displayBounds.center, (displayBounds.extents * spawnableDisplayIns.transform.localScale.y));
        //bottomDistance = Vector3.Distance(spawnableDisplayIns.transform.localPosition, (spawnableMesh.bounds.extents.y * spawnableDisplayIns.transform.localScale));
        float pivotCenterDistance = Vector3.Distance(spawnableDisplayIns.transform.position, displayBounds.center);
        //if the pivot is closer to the bottom bounds than the center, need to subtract the difference between the two instead of adding it
        pivotCenterDistance *= spawnableDisplayIns.transform.position.y > displayBounds.center.y ? 1.0f : -1.0f;

        bottomDistance = displayBounds.extents.y + pivotCenterDistance;

        //disable what we don't need
        MeshRenderer[] displayObjRenderers = spawnableDisplayIns.GetComponentsInChildren<MeshRenderer>();
        Behaviour[] displayBehaviours = spawnableDisplayIns.GetComponentsInChildren<Behaviour>();
        Collider[] displayColliders = spawnableDisplayIns.GetComponentsInChildren<Collider>();

        Color displayColor = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.1f);
        Material displayMaterial = (Material)Resources.Load("Materials/DisplaySpawnableMaterial", typeof(Material));

        for(int i = 0; i < displayObjRenderers.Length; i++)
        {
            if (displayMaterial != null)
            {
                displayObjRenderers[i].material = displayMaterial;
            }
        }

        for(int i = 0; i < displayBehaviours.Length; i++)
        {
            //can't remove networkidentities
            if (!displayBehaviours[i].GetType().ToString().Equals("UnityEngine.Networking.NetworkIdentity"))
            {
                Destroy(displayBehaviours[i]);
            }
        }

        for(int i = 0; i < displayColliders.Length; i++)
        {
            Destroy(displayColliders[i]);
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
                spawnableDisplayIns.transform.position = hit.point;
                //spawnableDisplayIns.transform.forward = spawnableRotation;
                spawnableDisplayIns.transform.up = hit.normal;
                //spawnableDisplayIns.transform.Rotate(spawnablePrefab.transform.rotation.eulerAngles);
                spawnableDisplayIns.transform.position += (hit.normal * bottomDistance);
                spawnableDisplayIns.transform.Rotate(spawnableDisplayIns.transform.up, currentRotation, Space.World);

                if (Vector3.Angle(hit.normal, Vector3.up) <= spawnAngleMax)
                {
                    //make color "valid" maybe so player knows they can spawn it
                    //maybe enable above so we're not CONSTANTLY enabling the object
                    if (!spawnableDisplayIns.activeSelf)
                    {
                        spawnableDisplayIns.SetActive(true);
                    }
                    canSpawn = true;
                }
                else
                {
                    Debug.Log("Invalid");
                    //make color "invalid" maybe
                    if (spawnableDisplayIns.activeSelf)
                    {
                        spawnableDisplayIns.SetActive(false);
                    }
                    canSpawn = false;
                }
            }
            else
            {
                if (spawnableDisplayIns.activeSelf)
                {
                    spawnableDisplayIns.SetActive(false);
                }
                canSpawn = false;
            }
        }
    }
}
