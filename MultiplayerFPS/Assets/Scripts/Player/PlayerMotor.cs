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
    private Vector3 currentDirection;

    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;
    private float distanceToGround;
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;

    [SerializeField]
    private LayerMask ignoreCollisionsLayer;

    private WeaponManager wepManager;
    private GameObject currentEquippedWeapon;

    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentDirection = transform.position;
        playerCollider = GetComponentInParent<CapsuleCollider>();
        wepManager = GetComponent<WeaponManager>();
    }

    void FixedUpdate()
    {
        currentDirection = (transform.position - currentDirection).normalized;
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
                Vector3 movePosition = rb.position + velocity * Time.fixedDeltaTime;
                rb.MovePosition(movePosition);
        }
    }

    public void Jump(float jumpForce)
    {
        if (IsGrounded())
        {
            rb.AddForce((Vector3.up * jumpForce), ForceMode.VelocityChange);
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
        return Physics.Raycast(playerCollider.transform.TransformPoint(Vector3.zero), Vector3.down, (0.15f));
    }

}
