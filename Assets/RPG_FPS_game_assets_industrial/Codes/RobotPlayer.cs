using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class RobotPlayer : BaseRobot
{
    // Start is called before the first frame update
    static RobotPlayer instance;

    private bool isGrounded;

    public float range;

    public float maxLookAngle = 40f;

    public Transform playBody;
    public Transform eye;

    public Vector3 targetPos;

    public float VrotateOffset;

    private float gravityValue = -9.8f;
    private Vector3 Velocity;

    public static RobotPlayer GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;
    }
    public List<WeaponBase> Weapons;
    public float RotateSense = 30;
    private WeaponBase curWeapon;
    private Animator animator;
    private int CurWeaponIdx = 0;
    private CharacterController cc;
    public float jumpSpeed = 10f;
    public float speed = 1;

    public AimIK aimIk;

    private bool handAim = false;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        playBody = GetComponent<Transform>();
        eye = GameObject.Find("FollowCamera").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        aimIk = GetComponent<AimIK>();
        aimIk.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetTarget();
        freeLook();
        freeMove();
        aim();
        fire();
    }

    void fire()
    {
        bool fireClick = Input.GetButton("Fire1");

        if (fireClick)
        {
            //有预瞄
            if (handAim)
            {
                animator.SetFloat("aiming", 1.5f);
                //无预瞄
            }
            else
            {
                aimIk.enabled = true;
                // animator.SetBool("toShoot",true);
                // animator.SetFloat("aiming",0f);
                animator.SetFloat("aiming", 1.5f);
            }
            aimIk.solver.target.position = targetPos;
        }

        if (!fireClick)
        {
            // animator.SetFloat("aiming",0f);
            if (handAim == false && animator.GetFloat("aiming") >= 0.5f)
            {
                // animator.SetBool("toShoot",false);
                animator.SetFloat("aiming", 0f);
                aimIk.enabled = false;
            }
        }
    }

    void aim()
    {
        bool aimClick = Input.GetMouseButtonDown(1);
        // 预瞄
        if (aimClick)
        {
            if (animator.GetFloat("aiming") >= 0.5f)
            {
                aimIk.enabled = false;
                handAim = false;
                animator.SetFloat("aiming", 0f);
            }
            else
            {
                aimIk.enabled = true;
                handAim = true;
                animator.SetFloat("aiming", 0.5f);
            }
            // animator.SetFloat("aiming",0f);
        }

        if (handAim || animator.GetFloat("aiming") >= 0.5f)
        {
            aimIk.enabled = true;
            aimIk.solver.target.position = targetPos;
        }
    }

    void freeLook()
    {
        float offsetH = Input.GetAxis("Mouse X"); // X的偏移量控制player水平方向的转动
        float offsetV = Input.GetAxis("Mouse Y"); // Y的偏移量控制eye垂直方向的转动

        playBody.Rotate(Vector3.up * RotateSense * offsetH * Time.deltaTime);

        VrotateOffset -= offsetV * RotateSense * Time.deltaTime;

        VrotateOffset = Mathf.Clamp(VrotateOffset, -20f, 20f);

        Quaternion currentQua = Quaternion.Euler(new Vector3(VrotateOffset, eye.localRotation.y, eye.localRotation.z));
        eye.localRotation = currentQua;
    }

    void freeMove()
    {
        isGrounded = cc.isGrounded;
        Velocity.x = 0;
        Velocity.z = 0;

        if (isGrounded && Velocity.y <= 0)
        {
            Velocity.y = -0.1f;
        }

        if (isGrounded)
        {
            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical = Input.GetAxis("Vertical");
            Debug.Log("Horizontal: " + Horizontal);
            Debug.Log("Vertical: " + Vertical);
            animator.SetFloat("Horizontal", Horizontal * 2);
            animator.SetFloat("Vertical", Vertical * 2);
            // if(Input.GetButtonDown("Jump")){
            //     animator.SetBool("Jump",true);
            // }
        }

        Velocity.y += gravityValue * Time.deltaTime;

        cc.Move(Velocity * Time.deltaTime);
    }

    public void SetTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(eye.transform.position, eye.transform.forward, out hit, range, 7))
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = eye.transform.position + (eye.transform.forward * range);
        }
        Debug.DrawRay(eye.transform.position, eye.transform.forward * range, Color.green);
    }

    private void turnDirect(int direction)
    {
        if (direction == 0)
        {
            animator.SetInteger("direction", direction);
            animator.SetFloat("walkspeed", 0f);
            return;
        }
        if (animator.GetInteger("direction") != direction)
        {
            animator.SetInteger("direction", direction);
            animator.SetFloat("walkspeed", 0f);
        }

    }
}
