using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(WeaponManager))]
public class PlayerController: MonoBehaviour
{

    [SerializeField]
    private float speed = 3f;
    [SerializeField]
    private float jumpForce = 3f;
    [SerializeField]
    private float lookSensitivity = 6f;

    private PlayerMotor motor;

    private WeaponManager wepMngr;


    //mask so our player can only collide with enviroment with this mask
    [SerializeField]
    private LayerMask environmentMask;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        wepMngr = GetComponent<WeaponManager>();
    }

    void Update()
    {

        //calculate movement as a 3d vector, doing Raw to get complete control of movement, without raw unity performs its own smoothing
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");
        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;
        Vector3 velocity = moveHorizontal + moveVertical;
        if (velocity.magnitude > 1.0f)
        {
            velocity.Normalize();
        }
        motor.Move(velocity * speed);

        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;
        motor.Rotate(rotation);

        float xRot = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRot * lookSensitivity;
        motor.RotateCamera(cameraRotationX);

 
        if (Input.GetButtonDown("Jump"))
        {
            motor.Jump(jumpForce);
        }
    }
}
