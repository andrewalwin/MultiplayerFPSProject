using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Pickup : NetworkBehaviour {

    [SerializeField]
    private float respawnTime;
    //[SerializeField]
    //private MeshRenderer pickupRenderer;
    [SerializeField]
    private Collider pickupTrigger;

    private bool pickedUp;

    private MeshRenderer[] pickupRenderers;

    private void Start()
    {
        pickupRenderers = GetComponentsInChildren<MeshRenderer>();
        pickedUp = false;
    }

    abstract protected void ApplyEffect(GameObject obj);

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.ToLower().Contains("player") && !pickedUp)
        {
            if (isServer)
            {
                ApplyEffect(col.gameObject);
            }
            pickedUp = true;
            pickupTrigger.enabled = false;
            foreach (MeshRenderer renderer in pickupRenderers)
            {
                renderer.enabled = false;
            }
            StartCoroutine(respawnPickup());
        }
    }

    public virtual IEnumerator respawnPickup()
    {
        yield return new WaitForSeconds(respawnTime);
        foreach (MeshRenderer renderer in pickupRenderers)
        {
            renderer.enabled = true;
        }
        pickupTrigger.enabled = true;
        pickedUp = false;
    }
}
