using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPlayer : BaseRobot
{
    // Start is called before the first frame update
    static RobotPlayer instance;

    private bool isGrounded;

    public float maxLookAngle=40f;

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
    public float WalkSpeed = 2;
    public float RotateSense = 30;
    private WeaponBase curWeapon;
    private Animator animator;
    private int CurWeaponIdx = 0;
    private CharacterController cc;
    public float jumpSpeed = 10f;
    public float speed = 1;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        playBody=GetComponent<Transform>();
        eye = GameObject.Find("FollowCamera").GetComponent<Transform>();
        animator =GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        freeLook();
        freeMove();
        fire();
    }

    void fire(){
        if(Input.GetButton("Fire1")){
            // turnDirect(0);
            animator.SetBool("shoot",true);
        }
        else
            animator.SetBool("shoot",false);
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

        // if(Input.GetButtonDown("Jump") && isGrounded)
        // {
        //     Debug.Log("Jumping");
        //     // Velocity.y += -jumpSpeed * gravityValue*Time.deltaTime;
        //     Velocity.y = Mathf.Sqrt(2*jumpSpeed * -gravityValue);
        //     isGrounded = false;
        //     animator.SetFloat("walkspeed",0f);
        // }

        // if(Input.GetAxis("Vertical")<=0 && isGrounded)
        //     animator.SetFloat("walkspeed",0.66f);


        // if(Input.GetAxis("Vertical")!=0 || Input.GetAxis("Horizontal")!=0){
        // }


        //     animator.SetFloat("walkspeed",speed);
        // }
        // else{
        //     if(isGrounded){
        //         if(Input.GetAxis("Vertical")>0 ){
        //             // 前进
        //             animator.SetBool("walkDirection",true);
        //             if (Input.GetKey(KeyCode.LeftShift))
        //                 speed = 1.5f;
        //             else
        //                 speed = 0.7f;
        //             animator.SetFloat("walkspeed",speed);
        //         }else if(Input.GetAxis("Vertical")<0){
        //             // 后退
        //             animator.SetBool("walkDirection",false);

        //         }else{
        //             //前进速度设为0
        //             speed = 0;
        //             // 后退速度设为0
        //             animator.SetFloat("walkspeed",speed);
        //         }

        //         if(Input.GetAxis("Horizontal")>0){
        //             // 右移
        //             if (Input.GetKey(KeyCode.LeftShift))
        //                 Velocity.x = 5f;
        //             else
        //                 Velocity.x = 1f;
        //         }else if(Input.GetAxis("Horizontal")<0){
        //             // 左移
        //             if(Input.GetKey(KeyCode.LeftShift))
        //                 Velocity.x = -5f;
        //             else
        //                 Velocity.x=-1f;
        //         }else
        //             Velocity.x=0f;

        //         if(Input.GetButtonDown("Jump")){
        //             if(Input.GetAxis("Vertical")>0){
        //                 if(Input.GetAxis("Horizontal")>0){
        //                     //前右跳
        //                 }else if(Input.GetAxis("Horizontal")<0){
        //                     //前左跳
        //                 }else{
        //                     //前跳
        //                 }

        //             }else if(Input.GetAxis("Vertical")<0){
        //                 if(Input.GetAxis("Horizontal")>0){
        //                     //后右跳
        //                 }else if(Input.GetAxis("Horizontal")<0){
        //                     //后左跳
        //                 }else{
        //                     //后跳
        //                 }
        //             }
        //         }
        //     }
        // }


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
        

        Vector3 moveDirection = new Vector3();

        Velocity.y += gravityValue*Time.deltaTime;

        // Vector3 moveDirection = transform.TransformDirection(Velocity);

        // animator.SetFloat("walkspeed",1.5f);

        if((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))){
            // animator.SetInteger("direction",0);
            // animator.SetFloat("walkspeed",0f);
            turnDirect(0);
        }else{
            if(Input.GetAxis("Vertical")>0){
                if(Input.GetAxis("Horizontal")>0){
                    // turnDirect(2);
                    moveDirection.x = 8*Input.GetAxis("Horizontal");
                }else if(Input.GetAxis("Horizontal")<0){
                    // turnDirect(8);
                    moveDirection.x = 8*Input.GetAxis("Horizontal");
                }
                turnDirect(1);
                animator.SetFloat("walkspeed",1.5f);
            }else if(Input.GetAxis("Vertical")<0){
                if(Input.GetAxis("Horizontal")>0){
                    // turnDirect(4);
                    moveDirection.x = 8*Input.GetAxis("Horizontal");
                }else if(Input.GetAxis("Horizontal")<0){
                    // turnDirect(6);
                    moveDirection.x = 8*Input.GetAxis("Horizontal");
                }
                turnDirect(3);
                animator.SetFloat("walkspeed",1.5f);
            }else if(Input.GetAxis("Horizontal")>0){
                turnDirect(2);
                animator.SetFloat("walkspeed",1.5f);
            }else if(Input.GetAxis("Horizontal")<0){
                turnDirect(4);     
                animator.SetFloat("walkspeed",1.5f);
            }else{
                // animator.SetInteger("direction",0);
                // animator.SetFloat("walkspeed",0f);
                turnDirect(0);
            }
        }

        moveDirection = transform.TransformDirection(moveDirection);
        cc.Move(Velocity*Time.deltaTime);
        cc.Move(moveDirection*Time.deltaTime);
    }


    private void OnAnimatorIK(int layerIndex) {
        
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
