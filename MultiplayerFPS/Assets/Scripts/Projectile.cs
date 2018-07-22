using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(PoolableObject))]
public class Projectile : MonoBehaviour {

    private float damage { get; set; }
    private float projectileLifetime { get; set; }
    [SerializeField]
    private GameObject projectileHitEffect { get; set; }

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
        projectileLifetime = 5f;
    }


    public void OnCollision(Collider collision)
    {
        Debug.Log(collision.transform.name);
        collision.gameObject.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);
        if (projectileHitEffect != null)
        {
            GameObject _hitEffect = (GameObject)Instantiate(projectileHitEffect, collision.transform.position, Quaternion.LookRotation(collision.transform.position.normalized));
        }

        this.gameObject.SetActive(false);
    }

    //destroys our projectile and returns it to our object pool using our PoolableObject component
    public virtual IEnumerator DestroyProjectile(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.gameObject.SetActive(false);
        poolableObject.RePoolObject();
    }



}
