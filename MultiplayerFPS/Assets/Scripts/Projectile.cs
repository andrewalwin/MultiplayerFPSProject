using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(PoolableObject))]
public class Projectile : MonoBehaviour {

    private float damage { get; set; }
    [SerializeField]
    private float knockbackAmount;
    private float projectileLifetime { get; set; }

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
        print(col.contacts[0].point);
        if (!col.gameObject.tag.ToLower().Contains("projectile"))
        {
            Health collidedObjectHealth;
            if ((collidedObjectHealth = col.gameObject.GetComponent<Health>()) != null)
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

            this.gameObject.SetActive(false);
            poolableObject.RePoolObject();
        }
    }

    public virtual IEnumerator DestroyProjectile(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
        poolableObject.RePoolObject();
    }
}
