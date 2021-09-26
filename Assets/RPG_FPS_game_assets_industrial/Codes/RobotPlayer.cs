using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPlayer : BaseRobot
{
    // Start is called before the first frame update
    static RobotPlayer instance;

    float rotateY = 0f;

    public float rotateSpeed=20f;
    public float moveSpeed = 20f;
    private bool isGrounded;

    public float maxLookAngle=60f;

    public Transform playBody;
    public Transform eye;

    public float VrotateOffset;

    private float gravityValue = -9.8f;
    private Vector3 Velocity;

    public static RobotPlayer GetInstance(){
        return instance;
    }
    private void Awake() {
        instance = this; 
    }
    public List<WeaponBase> Weapons;
    public float WalkSpeed = 10;
    private WeaponBase curWeapon;
    private Animator animator;
    private int CurWeaponIdx = 0;
    private CharacterController cc;
    public float jumpSpeed = 50f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        playBody=GetComponent<Transform>();
        eye = GameObject.Find("Camera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        freeLook();
        freeMove();
    }

    void freeLook(){
        float offsetH = Input.GetAxis("Mouse X"); // X的偏移量控制player水平方向的转动
        float offsetV = Input.GetAxis("Mouse Y"); // Y的偏移量控制eye垂直方向的转动

        playBody.Rotate(Vector3.up*moveSpeed*offsetH*Time.deltaTime);

        VrotateOffset -= offsetV*moveSpeed*Time.deltaTime;

        VrotateOffset =  Mathf.Clamp(VrotateOffset,-30f,30f);

        Quaternion currentQua = Quaternion.Euler(new Vector3(VrotateOffset,eye.localRotation.y,eye.localRotation.z));
        eye.localRotation = currentQua;
    }

    void freeMove(){
        isGrounded = cc.isGrounded;
        Vector3 moveDirection = Vector3.zero;
        moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= moveSpeed;
        cc.Move(moveDirection*Time.deltaTime);

        if(isGrounded && Velocity.y<=0){
            Velocity.y = -0.1f;
        }
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jumping");
            Velocity.y += -jumpSpeed * gravityValue*Time.deltaTime;
        }
        Velocity.y += gravityValue * Time.deltaTime;
        cc.Move(Velocity*Time.deltaTime);
    }
}
