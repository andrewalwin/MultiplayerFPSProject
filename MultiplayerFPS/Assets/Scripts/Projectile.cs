using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Projectile : MonoBehaviour {

    private float damage { get; set; }
    private float destroyDelay { get; set; }
    [SerializeField]
    private GameObject projectileHitEffect { get; set; }

    public Projectile()
    {
        damage = 10f;
        destroyDelay = 5f;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        collision.gameObject.SendMessage("Damage", damage);
        if(projectileHitEffect != null)
        {
            GameObject _hitEffect = (GameObject)Instantiate(projectileHitEffect, collision.transform.position, Quaternion.LookRotation(collision.transform.position.normalized));
        }

        DestroyProjectile(0);
    }

    //destroys our projectile, can have special functionality here, like a grenade exploding when it gets destroyed after x seconds
    public virtual void DestroyProjectile(float delay)
    {
        if(this.gameObject != null)
        {
            //Destroy(this.gameObject, delay);
            this.gameObject.SetActive(false);
        }
    }



}
