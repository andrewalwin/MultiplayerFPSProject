using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleProjectile : MonoBehaviour {

    [SerializeField]
    private float magnitude = 0.01f;
    [SerializeField]
    private float frequency = 4f;
    private Vector3 axis;

    [SerializeField]
    private float speed = 0.005f;
    private Vector3 velocity;
    private Rigidbody rb;

    [SerializeField]
    private float noiseMagnitude = 6f;

    //how many times we should scale our object by (not in units)
    [SerializeField]
    private float scaleFactor = 3f;
    //how many seconds it takes to scale our object
    [SerializeField]
    private float scaleTime = 4f;

    //the originalScale of our object
    private Vector3 originalScale;

    // Use this for initialization
    void Start () {

        velocity = transform.forward.normalized;
        rb = GetComponent<Rigidbody>();

        originalScale = transform.localScale;

        axis = transform.up;
    }

    // Update is called once per frame
    void Update () {
        //velocity = origVelocity;

        //calculate how much we need to scale this frame
        float scaleAmount = ((((originalScale.x * scaleFactor)) - originalScale.x) / scaleTime) * Time.deltaTime;

            //only seperating these into 2 conditionals for scaling the height and width in different times, if theyre the same time just do both //in 1 conditional (wouldnt need sepeerate heightScale and widthScale)
            if (transform.localScale.y < scaleFactor * originalScale.y)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + (1.5f * scaleAmount), transform.localScale.z);
                //update our currentScale
            }
            if (transform.localScale.x < scaleFactor * originalScale.x)
            {
                //do we need to scale both x and z (think we do)? try without too
                transform.localScale = new Vector3(transform.localScale.x + scaleAmount, transform.localScale.y, transform.localScale.z + scaleAmount);
                //update our currentScale
  
        }
        
        //moving last here so our localscale vector doesnt mess up movement
        //rb.MovePosition(rb.position + velocity * Time.deltaTime);
        Vector3 pos = rb.position + velocity * Time.deltaTime;
        transform.position = (pos + axis * Mathf.Sin(Time.time * frequency) * magnitude);


    }
}
