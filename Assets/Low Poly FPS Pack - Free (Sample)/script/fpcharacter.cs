using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpcharacter : MonoBehaviour {

private CharacterController characterController;
private Transform characterTransform;
public float Gravity = 9.8f;
public float MovementSpeed;

public float JumpHeight;

public float mouseSensitivity = 100f;

public float xRotation = 0f;

public float yRotation = 0f;

private Vector3 movementDirection;
	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController>();
		characterTransform = GetComponent<Transform>();
		Cursor.lockState = CursorLockMode.Locked;
	}
	// Update is called once per frame
	void Update () {
		if(characterController.isGrounded){
			var tmp_Horizontal = Input.GetAxis("Horizontal");	
			var tmp_Vertical = Input.GetAxis("Vertical");
			movementDirection =
				characterTransform.TransformDirection(new Vector3(tmp_Horizontal,0,tmp_Vertical));
			// characterController.SimpleMove(tmp_MovementDirection*MovementSpeed);
			if(Input.GetButtonDown("Jump")){
				movementDirection.y=JumpHeight;
			}
		}

		var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		characterTransform.Rotate(Vector3.up*mouseX);
		var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		movementDirection.y -=Gravity*Time.deltaTime; 
		characterController.Move(movementDirection*Time.deltaTime*MovementSpeed);
		xRotation -= mouseY;
		yRotation += mouseX;
		xRotation = Mathf.Clamp(xRotation,-90f,90f);
		transform.localRotation=Quaternion.Euler(xRotation,yRotation,0f);
	}
}
