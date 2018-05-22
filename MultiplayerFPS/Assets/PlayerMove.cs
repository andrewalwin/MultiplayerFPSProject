using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 6f;

    //bool controlling if player can move
    private bool canMove = true;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    //dashing stuff
    [SerializeField]
    private float dashSpeed = 6f;
    private float dashKeyCooldown = 0.3f;
    private KeyCode dashKey;
    private bool dashing = false;
    private float dashTime = 0.3f;

    //jump bool
    private bool canJump = true;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Rigidbody rb;

    [SerializeField]
    private Camera cam;

    //weapon stuff
    private WeaponManagerOff weaponManager;

    //UI Stuff
    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        weaponManager = GetComponent<WeaponManagerOff>();

        //ui stuff
        playerUIInstance = Instantiate(playerUIPrefab);
        playerUIInstance.name = playerUIPrefab.name;
    }
    
	
	// Update is called once per frame
	void Update () {

        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(vKey))
            {
                if (vKey == dashKey && dashKeyCooldown > 0)
                {
                    PlayerDash(vKey);
                }
                else
                {
                    dashKeyCooldown = 0.5f;
                    dashKey = vKey;
                }
            }
        }

        if(dashKeyCooldown > 0)
        {
            dashKeyCooldown -= Time.deltaTime;
        }
        else
        {
            dashKeyCooldown = 0f;
        }

        if (canMove)
        {
            //calculate movement as a 3d vector, doing Raw to get complete control of movement, without raw unity performs its own smoothing
            float xMov = Input.GetAxis("Horizontal");
            float zMov = Input.GetAxis("Vertical");

            //transform.right instead of vector3.right is local, takes into account our current rotation
            Vector3 moveHorizontal = transform.right * xMov;
            Vector3 moveVertical = transform.forward * zMov;

            //normalize so theres no varying speed, always normal vector(just for direction, speed is for the actual speed)
            Vector3 velocity = (moveHorizontal + moveVertical) * speed;

            Move(velocity);

            //Calculate rotational vector
            //want to rotate around the y when you move the mouse x 
            //Only want the player rotated in the x, since we want up and down to only affect the camera (dont move toward the mouse when you look up and down)
            float yRot = Input.GetAxisRaw("Mouse X");

            Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

            //Apply rotation
            Rotate(rotation);

            //calculate camera rotation
            float xRot = Input.GetAxisRaw("Mouse Y");

            float cameraRotationX = xRot * lookSensitivity;

            //Apply camera rotation
            RotateCamera(cameraRotationX);

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }

        PerformMovement();
        PerformRotation();

        if (dashing)
        {
            dashTime -= Time.deltaTime;
        }
        if(dashTime <= 0)
        {
            dashTime = 0.3f;
            velocity = Vector3.zero;
            canMove = true;
            dashing = false;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            weaponManager.SwitchWeapon();
        }
    }

    public void Move(Vector3 velocityMove)
    {
        //velocityMove is passed in from our controller
        velocity = velocityMove;
    }

    public void Jump()
    {
        if (canJump)
        {
            canJump = false;
            rb.velocity += new Vector3(0f, 3f, 0f);
        }
    }

    public void Rotate(Vector3 rotationMove)
    {
        rotation = rotationMove;
    }

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            //movePosition does collision checks on the way
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
        }
    }

    void PerformRotation()
    {
        //rotations go through quaternion system, unity will handle it for us
        //Quaternion.Euler takes our euler and turns it into a quaternion for us
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        //if you want the tilt to be optional
        if (cam != null)
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

    public void Grounded()
    {
        if (!canJump) { canJump = true; }
    }
    
    public void PlayerDash(KeyCode key)
    {
        if(key == KeyCode.W)
        {
            canMove = false;
            dashing = true;
            velocity = transform.forward * dashSpeed;
        }
        else if (key == KeyCode.A)
        {
            canMove = false;
            dashing = true;
            velocity = -transform.right * dashSpeed;
        }
        else if (key == KeyCode.D)
        {
            canMove = false;
            dashing = true;
            velocity = transform.right * dashSpeed;

        }
        else if (key == KeyCode.S)
        {
            canMove = false;
            dashing = true;
            velocity = -transform.forward * dashSpeed;
        }
        Move(velocity);
        dashKey = 0;

    }
}
