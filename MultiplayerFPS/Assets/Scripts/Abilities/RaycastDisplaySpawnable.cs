using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RaycastDisplaySpawnable : MonoBehaviour {

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
    [SerializeField]
    private float overlapBoxSize;
    private float bottomDistance = 0f;
    private float currentRotation = 0f;

    [SerializeField]
    private LayerMask ignoreCollisions;

    private bool isDisplaying = false;
	private bool canSpawn;

    [SerializeField]
    private KeyCode abilityKey;

    private Material validDisplayMaterial;
    private Material invalidDisplayMaterial;

    private MeshRenderer[] displayObjRenderers;

    private Bounds displayBounds;
    private Bounds colliderBounds;

    void Start()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
        if (spawnablePrefab != null)
        {
            SetupDisplayable();
        }

        validDisplayMaterial = (Material)Resources.Load("Materials/DisplaySpawnableMaterial", typeof(Material));
        invalidDisplayMaterial = (Material)Resources.Load("Materials/InvalidDisplaySpawnableMaterial", typeof(Material));
    }

    void Update()
    {
        if (spawnableDisplayIns != null)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                currentRotation = 0f;
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

            if (Input.GetMouseButton(0))
            {
                if(isDisplaying && canSpawn)
                {
                    isDisplaying = false;
                    GameObject spawnedIns = GameObject.Instantiate(spawnablePrefab, spawnableDisplayIns.transform.position, spawnableDisplayIns.transform.rotation);
                    Spawnable spawnableBehav = spawnedIns.GetComponent<Spawnable>();
                    if(spawnableBehav != null)
                    {
                        Vector3 floorPos = spawnedIns.transform.position + (spawnedIns.transform.up.normalized * bottomDistance * -1);
                        spawnableBehav.OnSpawn(floorPos);
                    }
                    
                }
            }
        }
    }

    private void SetupDisplayable()
    {
        spawnableDisplayIns = GameObject.Instantiate(spawnablePrefab, new Vector3(0, 0, 0), spawnablePrefab.transform.rotation);
        spawnableDisplayIns.transform.localScale = spawnablePrefab.transform.localScale;

        spawnableDisplayIns.SetActive(false);

        displayBounds = spawnableDisplayIns.GetComponentInChildren<Renderer>().bounds;
        colliderBounds = spawnableDisplayIns.GetComponentInChildren<Collider>().bounds;

        Renderer[] displayRenderers = spawnableDisplayIns.GetComponentsInChildren<Renderer>();
        Collider[] displayColliders = spawnableDisplayIns.GetComponentsInChildren<Collider>();
        if (displayRenderers.Length > 0)
        {
            foreach (Renderer rend in displayRenderers)
            {
                if (rend != spawnableDisplayIns.GetComponent<Renderer>())
                {
                    displayBounds.Encapsulate(rend.bounds);
                }
            }

            float pivotCenterDistance = Vector3.Distance(spawnableDisplayIns.transform.position, displayBounds.center);
            pivotCenterDistance *= spawnableDisplayIns.transform.position.y > displayBounds.center.y ? 1.0f : -1.0f;

            bottomDistance = displayBounds.extents.y + pivotCenterDistance;
        }
        else
        {
            foreach (Collider col in displayColliders)
            {
                if(col != spawnableDisplayIns.GetComponent<Collider>())
                {
                    colliderBounds.Encapsulate(col.bounds);
                }
            }

            float pivotCenterDistance = Vector3.Distance(spawnableDisplayIns.transform.position, colliderBounds.center);
            pivotCenterDistance *= spawnableDisplayIns.transform.position.y > colliderBounds.center.y ? 1.0f : -1.0f;

            bottomDistance = displayBounds.extents.y + pivotCenterDistance;
        }

        //disable what we don't need
        displayObjRenderers = spawnableDisplayIns.GetComponentsInChildren<MeshRenderer>();
        Behaviour[] displayBehaviours = spawnableDisplayIns.GetComponentsInChildren<Behaviour>();

        Color displayColor = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.1f);

        for(int i = 0; i < displayObjRenderers.Length; i++)
        {
            if (validDisplayMaterial != null)
            {
                displayObjRenderers[i].material = validDisplayMaterial;
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
                spawnableDisplayIns.transform.up = hit.normal;
                spawnableDisplayIns.transform.position += (hit.normal * bottomDistance);
                spawnableDisplayIns.transform.Rotate(spawnableDisplayIns.transform.up, currentRotation, Space.World);

                if (Vector3.Angle(hit.normal, Vector3.up) <= spawnAngleMax)
                {
                    if (!spawnableDisplayIns.activeSelf)
                    {
                        spawnableDisplayIns.SetActive(true);
                    }
                    if(displayObjRenderers[0].material != validDisplayMaterial)
                    {
                        for(int i = 0; i < displayObjRenderers.Length; i++)
                        {
                            displayObjRenderers[i].material = validDisplayMaterial;
                        }
                    }
                    canSpawn = true;
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

            else
            {
                if (spawnableDisplayIns.activeSelf)
                {
                    spawnableDisplayIns.SetActive(false);
                }
                canSpawn = false;
            }

            //check if our object collides with anything, disabling spawning, and making it display as invalid
            Collider[] intersectedColliders = Physics.OverlapBox(spawnableDisplayIns.transform.TransformPoint(displayBounds.center), displayBounds.extents * overlapBoxSize, spawnableDisplayIns.transform.rotation);

            if (intersectedColliders.Length > 0 && displayObjRenderers[0].material != invalidDisplayMaterial){
                for (int i = 0; i < displayObjRenderers.Length; i++)
                {
                    displayObjRenderers[i].material = invalidDisplayMaterial;
                    canSpawn = false;
                }
            }
        }
    }
}
