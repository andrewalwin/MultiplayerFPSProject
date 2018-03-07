using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 6f;
    [SerializeField]
    private float thrusterForce = 1000f;

    //all our spring settings so they show up in inspector
    [Header("Sprint settings:")]
    [SerializeField]
    private JointDriveMode jointMode = JointDriveMode.Position;
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;

    void Start()
    {
        //dont have to do checking/error handling here due to our requirecomponent
        motor = GetComponent<PlayerMotor>();
        //require a joint and set it here instead of in inspector
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
    }

    void Update()
    {
        //calculate movement as a 3d vector, doing Raw to get complete control of movement, without raw unity performs its own smoothing
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        //transform.right instead of vector3.right is local, takes into account our current rotation
        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        //normalize so theres no varying speed, always normal vector(just for direction, speed is for the actual speed)
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

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
        if (Input.GetButton("Jump"))
        {
            //if we dont press jump, then we're just applying 0 up, which is fine
            _thrusterForce = Vector3.up * thrusterForce;
            //want to set our joint sprint to 0 so it doesnt pull us down
            SetJointSettings(0f);
        }
        else
        {
            SetJointSettings(jointSpring);
        }

        motor.ApplyThruster(_thrusterForce);


    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            mode = jointMode,
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }


}
