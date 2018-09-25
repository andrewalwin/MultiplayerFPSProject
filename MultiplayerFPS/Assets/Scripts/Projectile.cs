using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
[RequireComponent(typeof(PoolableObject))]
public class Projectile : NetworkBehaviour {
    [SerializeField]
    private float damage { get; set; }
    [SerializeField]
    private float knockbackAmount;
    private float projectileLifetime { get; set; }

    public bool dealDamage = true;

    [SerializeField]
    private GameObject projectileHitEffect;

    private PoolableObject poolableObject;

    private void Start()
    {
        poolableObject = gameObject.GetComponent<PoolableObject>();
    }

    private void OnEnable()
    {
        StopCoroutine(DestroyProjectile(projectileLifetime));
        StartCoroutine(DestroyProjectile(projectileLifetime));
    }
    public Projectile()
    {
        damage = 10f;
        knockbackAmount = 1f;
        projectileLifetime = 10f;
    }


    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag.ToLower().Contains("projectile"))
        {
            return;
        }
        Health collidedObjectHealth;
        collidedObjectHealth = col.gameObject.GetComponent<Health>();

        if (collidedObjectHealth != null && dealDamage)
        {
            collidedObjectHealth.Damage(damage);
        }

        if (projectileHitEffect != null)
        {
            GameObject _hitEffect = (GameObject)Instantiate(projectileHitEffect, col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal));
        }

        if (col.gameObject.tag.Contains("Physics") && col.gameObject.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("PHYSICS");
            Vector3 worldTravelDirection = transform.TransformVector(gameObject.transform.forward).normalized;
            if (col.gameObject.GetComponent<Rigidbody>() != null)
            {
                col.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(transform.InverseTransformDirection(worldTravelDirection) * knockbackAmount, col.contacts[0].point, ForceMode.Impulse);
            }
        }

        dealDamage = true;
        this.gameObject.SetActive(false);
        poolableObject.RePoolObject();
    }

    public virtual IEnumerator DestroyProjectile(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
        poolableObject.RePoolObject();
    }
}
