using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor_Old : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    //how far our camera can rotate
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    public void Move(Vector3 velocityMove)
    {
        //velocityMove is passed in from our controller
        velocity = velocityMove;
    }

    void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            //movePosition does collision checks on teh way
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if (thrusterForce != Vector3.zero)
        {
            //want this to be constant, no matter the players mass, which ForceMode.Acceleration does
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

   public void Rotate(Vector3 rotationMove)
    {
        rotation = rotationMove;
    }

    void PerformRotation()
    {
        //rotations go through quaternion system, unity will handle it for us
        //Quaternion.Euler takes our euler and turns it into a quaternion for us
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        //if you want the tilt to be optional
        if(cam!= null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            //apply to transform
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    public void RotateCamera(float cameraRotationMoveX)
    {
        cameraRotationX = cameraRotationMoveX;
    }

    public void ApplyThruster(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }
}
