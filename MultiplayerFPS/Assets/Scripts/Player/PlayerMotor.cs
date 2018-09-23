using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{

    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;

    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private float distanceToGround;
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;
    private BoxCollider playerCollider;

    [SerializeField]
    private LayerMask ignoreCollisionsLayer;

    private WeaponManager wepManager;
    private GameObject currentEquippedWeapon;

    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponentInParent<BoxCollider>();
        wepManager = GetComponent<WeaponManager>();
    }

    void FixedUpdate()
    {
        //Vector3 ray = transform.TransformDirection(Vector3.down) * (playerCollider.bounds.extents.y + 0.1f);
        //Debug.DrawRay(playerCollider.transform.TransformPoint(Vector3.zero), ray, Color.green, 2, false);
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
        if (velocity != Vector3.zero)
        {
            //movePosition does collision checks on the way
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    public void Jump(float jumpForce)
    {
        Debug.Log("GROUNDED? : " + IsGrounded());
        if (IsGrounded())
        {
            Debug.Log("JUMPING");
            //velocity += new Vector3(0, jumpForce * 10 , 0);
            rb.AddForce((Vector3.up * jumpForce), ForceMode.Impulse);
        }
    }

    public void Rotate(Vector3 rotationMove)
    {
        rotation = rotationMove;
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (wepManager.GetCurrentWeaponIns() != null)
        {
            wepManager.GetCurrentWeaponIns().transform.rotation = (rb.rotation * Quaternion.Euler(rotation));
        }

        //if you want the tilt to be optional
        if (cam != null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            //apply to transform
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
            if (wepManager.GetCurrentWeaponIns() != null)
            {
                wepManager.GetCurrentWeaponIns().transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
            }
        }
    }

    public void RotateCamera(float cameraRotationMoveX)
    {
        cameraRotationX = cameraRotationMoveX;
    }

    public bool IsGrounded()
    {
        Debug.Log(transform.position.y + ", " + distanceToGround + ", " + Physics.defaultContactOffset);
        return Physics.Raycast(playerCollider.transform.TransformPoint(Vector3.zero), Vector3.down, (Physics.defaultContactOffset));
    }

}
