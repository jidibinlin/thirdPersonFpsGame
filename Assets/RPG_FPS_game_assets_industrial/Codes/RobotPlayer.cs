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

    public float maxLookAngle=40f;

    public Transform playBody;
    public Transform eye;

    public Vector3 targetPos;

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
    public float RotateSense = 30;
    private WeaponBase curWeapon;
    private Animator animator;
    private int CurWeaponIdx = 0;
    private CharacterController cc;
    public float jumpSpeed = 10f;
    public float speed = 1;

    public AimIK aimIk;

    private bool handAim=false;

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        playBody=GetComponent<Transform>();
        eye = GameObject.Find("FollowCamera").GetComponent<Transform>();
        animator =GetComponent<Animator>();
        aimIk = GetComponent<AimIK>();
        aimIk.enabled=false;
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

    // void fire(){
    //     bool fireClick = Input.GetButton("Fire1");

    //     if(fireClick){
    //         //有预瞄
    //         if(handAim){
    //             animator.SetFloat("aiming",1f);
    //         //无预瞄
    //         }else{
    //             aimIk.enabled = true;
    //             animator.SetBool("toShoot",true);
    //             animator.SetFloat("aiming",1f);
    //         }
    //         aimIk.solver.target.position = targetPos;
    //     }

    //     if(!fireClick){
    //         animator.SetFloat("aiming",0f);
    //         if(handAim==false && animator.GetBool("toShoot")==true){
    //             animator.SetBool("toShoot",false);
    //             aimIk.enabled = false;
    //         }
    //     }
    // }
    void fire(){
        bool fireClick = Input.GetButton("Fire1");

        if(fireClick){
            //有预瞄
            if(handAim){
                animator.SetFloat("aiming",1.5f);
            //无预瞄
            }else{
                aimIk.enabled = true;
                // animator.SetBool("toShoot",true);
                // animator.SetFloat("aiming",0f);
                animator.SetFloat("aiming",1.5f);
            }
            aimIk.solver.target.position = targetPos;
        }

        if(!fireClick){
            // animator.SetFloat("aiming",0f);
            if(handAim==false && animator.GetFloat("aiming")>=0.5f){
                // animator.SetBool("toShoot",false);
                animator.SetFloat("aiming",0f);
                aimIk.enabled = false;
            }
        }
    }

    // void aim(){
    //     bool aimClick = Input.GetMouseButtonDown(1); 
    //     // 预瞄
    //     if(aimClick){
    //         if(animator.GetBool("toShoot")==true){
    //             aimIk.enabled = false; 
    //             handAim = false;
    //             animator.SetBool("toShoot",false);
    //         }else{
    //             aimIk.enabled = true;
    //             handAim = true;
    //             animator.SetBool("toShoot",true);
    //         }
    //         animator.SetFloat("aiming",0f);
    //     }

    //     if(handAim || animator.GetBool("toShoot")==true){
    //         aimIk.enabled = true;
    //         aimIk.solver.target.position = targetPos;
    //     }
    // }
    void aim(){
        bool aimClick = Input.GetMouseButtonDown(1); 
        // 预瞄
        if(aimClick){
            if(animator.GetFloat("aiming")>=0.5f){
                aimIk.enabled = false; 
                handAim = false;
                animator.SetFloat("aiming",0f);
            }else{
                aimIk.enabled = true;
                handAim = true;
                animator.SetFloat("aiming",0.5f);
            }
            // animator.SetFloat("aiming",0f);
        }

        if(handAim || animator.GetFloat("aiming")>=0.5f){
            aimIk.enabled = true;
            aimIk.solver.target.position = targetPos;
        }
    }

    void freeLook(){
        float offsetH = Input.GetAxis("Mouse X"); // X的偏移量控制player水平方向的转动
        float offsetV = Input.GetAxis("Mouse Y"); // Y的偏移量控制eye垂直方向的转动

        playBody.Rotate(Vector3.up*RotateSense*offsetH*Time.deltaTime);

        VrotateOffset -= offsetV*RotateSense*Time.deltaTime;

        VrotateOffset =  Mathf.Clamp(VrotateOffset,-20f,20f);

        Quaternion currentQua = Quaternion.Euler(new Vector3(VrotateOffset,eye.localRotation.y,eye.localRotation.z));
        eye.localRotation = currentQua;
    }

    void freeMove(){
        isGrounded = cc.isGrounded;
        Velocity.x=0;
        Velocity.z=0;
        // Vector3 moveDirection = Vector3.zero;
        // moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        // moveDirection = transform.TransformDirection(moveDirection);
        // moveDirection *= WalkSpeed;
        // cc.Move(moveDirection*Time.deltaTime);

        if(isGrounded && Velocity.y<=0){
            Velocity.y = -0.1f;
        }



        // if(Input.GetAxis("Vertical")>0 && isGrounded && Input.GetKey(KeyCode.LeftShift))
        // {
        //     speed = 1.5f;
        //     animator.SetFloat("walkspeed",speed);
        // }else if(Input.GetAxis("Vertical")>0 && isGrounded){
        //     speed = 0.7f;
        //     animator.SetFloat("walkspeed",speed);
        // }

        // if(Input.GetAxis("Horizontal")!=0 && isGrounded){
        //     animator.SetFloat("walkspeed",0.2f);
        //     Debug.Log("Horizontal move");
        // }

        if(isGrounded){
            float Horizontal = Input.GetAxis("Horizontal");
            float Vertical  = Input.GetAxis("Vertical");
            Debug.Log("Horizontal: "+Horizontal);
            Debug.Log("Vertical: "+Vertical);
            animator.SetFloat("Horizontal",Horizontal*2);
            animator.SetFloat("Vertical",Vertical*2);
            if(Input.GetButtonDown("Jump")){
                animator.SetBool("Jump",true);
            }
        }

        

        // Vector3 moveDirection = new Vector3();

        Velocity.y += gravityValue*Time.deltaTime;

        // Vector3 moveDirection = transform.TransformDirection(Velocity);

        // animator.SetFloat("walkspeed",1.5f);

        // if((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))){
        //     // animator.SetInteger("direction",0);
        //     // animator.SetFloat("walkspeed",0f);
        //     turnDirect(0);
        // }else{
        //     if(Input.GetAxis("Vertical")>0){
        //         if(Input.GetAxis("Horizontal")>0){
        //             // turnDirect(2);
        //             moveDirection.x = 8*Input.GetAxis("Horizontal");
        //         }else if(Input.GetAxis("Horizontal")<0){
        //             // turnDirect(8);
        //             moveDirection.x = 8*Input.GetAxis("Horizontal");
        //         }
        //         turnDirect(1);
        //         animator.SetFloat("walkspeed",1.5f);
        //     }else if(Input.GetAxis("Vertical")<0){
        //         if(Input.GetAxis("Horizontal")>0){
        //             // turnDirect(4);
        //             moveDirection.x = 8*Input.GetAxis("Horizontal");
        //         }else if(Input.GetAxis("Horizontal")<0){
        //             // turnDirect(6);
        //             moveDirection.x = 8*Input.GetAxis("Horizontal");
        //         }
        //         turnDirect(3);
        //         animator.SetFloat("walkspeed",1.5f);
        //     }else if(Input.GetAxis("Horizontal")>0){
        //         turnDirect(2);
        //         animator.SetFloat("walkspeed",1.5f);
        //     }else if(Input.GetAxis("Horizontal")<0){
        //         turnDirect(4);     
        //         animator.SetFloat("walkspeed",1.5f);
        //     }else{
        //         // animator.SetInteger("direction",0);
        //         // animator.SetFloat("walkspeed",0f);
        //         turnDirect(0);
        //     }
        // }

        // moveDirection = transform.TransformDirection(moveDirection);
        cc.Move(Velocity*Time.deltaTime);
        // cc.Move(moveDirection*Time.deltaTime);
    }

    public void SetTarget(){
        RaycastHit hit;
        if(Physics.Raycast(eye.transform.position,eye.transform.forward,out hit,range,7)){
            targetPos=hit.point;
        }
        else{
            targetPos = eye.transform.position+(eye.transform.forward*range);
        }
        Debug.DrawRay(eye.transform.position,eye.transform.forward*range,Color.green);
    }

    private void turnDirect(int direction){
        if(direction == 0){
            animator.SetInteger("direction",direction);
            animator.SetFloat("walkspeed",0f);
            return;
        }
        if(animator.GetInteger("direction")!=direction){
            animator.SetInteger("direction",direction);
            animator.SetFloat("walkspeed",0f);
        }

    }
}
