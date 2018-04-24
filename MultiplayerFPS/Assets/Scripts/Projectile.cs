using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Projectile : MonoBehaviour {

    private float damage { get; set; }
    private float speed { get; set; }
    private Vector3 direction { get; set; }
    private float destroyDelay { get; set; }
    [SerializeField]
    private GameObject projectileHitEffect { get; set; }

    public Projectile(float _damage, float _speed, Vector3 _direction)
    {
        damage = _damage;
        speed = _speed;
        direction = _direction;
    }

    public Projectile()
    {
        damage = 10f;
        speed = 20f;
        destroyDelay = 5f;
    }

    void Update()
    {
        DestroyProjectile(destroyDelay);

    }

    public virtual void OnCollisionEnter(Collision collision)
    {
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
            Destroy(this.gameObject, delay);
        }
    }



}
