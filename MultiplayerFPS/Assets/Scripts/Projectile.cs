using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Projectile : MonoBehaviour {

    private float damage { get; set; }
    private float projectileLifetime { get; set; }
    [SerializeField]
    private GameObject projectileHitEffect { get; set; }

    private void Start()
    {
        StartCoroutine(DestroyProjectile(projectileLifetime));
    }

    public Projectile()
    {
        damage = 10f;
        projectileLifetime = 5f;
    }


    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.transform.name);
        collision.gameObject.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);
        if (projectileHitEffect != null)
        {
            GameObject _hitEffect = (GameObject)Instantiate(projectileHitEffect, collision.transform.position, Quaternion.LookRotation(collision.transform.position.normalized));
        }

        this.gameObject.SetActive(false);
    }

    //destroys our projectile, can have special functionality here, like a grenade exploding when it gets destroyed after x seconds
    public virtual IEnumerator DestroyProjectile(float delay)
    {
        yield return new WaitForSeconds(delay);

        this.gameObject.SetActive(false);
    }



}
