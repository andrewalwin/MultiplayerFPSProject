using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor_Old))]
public class PlayerController_Old : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 6f;
    [SerializeField]
    private float thrusterForce = 1000f;
    [SerializeField]
    private float thrusterFuelUseSpeed = 1f;   //thruster lasts for this amount of seconds
    [SerializeField]
    private float thrusterRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    //mask so our player can only collide with enviroment with this mask
    [SerializeField]
    private LayerMask environmentMask;

    //all our spring settings so they show up in inspector
    [Header("Sprint settings:")]

    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;
    
    //component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start()
    {
        //dont have to do checking/error handling here due to our requirecomponent
        motor = GetComponent<PlayerMotor>();
        //require a joint and set it here instead of in inspector
        joint = GetComponent<ConfigurableJoint>();

        animator = GetComponent<Animator>();
        SetJointSettings(jointSpring);
    }

    void Update()
    {
        //raycasting so our spring joint performs correctly over surfaces
        RaycastHit _hit;
        if(Physics.Raycast(transform.position, Vector3.down, out _hit))
        {
            //want to add offset on our y so we're always the same amount above the surface below us
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        //calculate movement as a 3d vector, doing Raw to get complete control of movement, without raw unity performs its own smoothing
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        //transform.right instead of vector3.right is local, takes into account our current rotation
        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        //normalize so theres no varying speed, always normal vector(just for direction, speed is for the actual speed)
        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        //Animate movement
        animator.SetFloat("ForwardVelocity", zMov);

        //Apply movement
        motor.Move(velocity);

        //Calculate rotational vector
        //want to rotate around the y when you move the mouse x 
        //Only want the player rotated in the x, since we want up and down to only affect the camera (dont move toward the mouse when you look up and down)
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

        //Apply rotation
        motor.Rotate(rotation);

        //calculate camera rotation
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * lookSensitivity;

        //Apply camera rotation
        motor.RotateCamera(cameraRotationX);

        Vector3 _thrusterForce = Vector3.zero;
        //Apply thruster force
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0)
        {
            thrusterFuelAmount -= thrusterFuelUseSpeed * Time.deltaTime;

            //only add thruster force if we have enough fuel so player cant hover forever
            if (thrusterFuelAmount >= 0.01f)
            {
                //if we dont press jump, then we're just applying 0 up, which is fine
                _thrusterForce = Vector3.up * thrusterForce;
                //want to set our joint sprint to 0 so it doesnt pull us down
                SetJointSettings(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterRegenSpeed * Time.deltaTime;
            SetJointSettings(jointSpring);
        }
        //make sure our thruster fuel amount doesnt go above 1
        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);



    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }


}
