using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class FPSMovement : MonoBehaviour
{
	
	//Public Var
	public float moveSpeed;
	
	//Private Var
	
	Vector3 translation;
    Vector3 velocity;
	Rigidbody RBody;
	Animator Anim;
	

	void Awake()
	{
		RBody = GetComponent<Rigidbody>();
		Anim = GetComponent<Animator>();
	}
	
	void Update()
	{
		KeyboardInputs();
	}
	
	void FixedUpdate()
	{
		MoveFPSPlayer();
	}
	

	void KeyboardInputs()
	{
		translation = Vector3.zero;

        // ZQSD or Arrow keys

        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) { translation += transform.forward;}
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { translation -= transform.forward;}
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { translation += transform.right;}
		if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) { translation -= transform.right;}
		
	}
	
	void MoveFPSPlayer()
        {
            //Move
            transform.localPosition += translation * moveSpeed;
           
        }
}
