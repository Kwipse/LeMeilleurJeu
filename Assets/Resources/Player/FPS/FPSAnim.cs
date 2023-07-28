using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FPSAnim : MonoBehaviour
{
	public bool IsGrounded;
	public bool IsMoving;
	
	Animator Anim;
	Rigidbody RBody;
	Vector3 position;
	
	
	void Awake()
    {
		Anim = GetComponent<Animator>();
		RBody = GetComponent<Rigidbody>();	
		
		position = RBody.position;
    }
	
	void Update()
	{

	}
	
	void FixedUpdate()
	{
		if (RBody.position == position)
		{
            Anim.SetBool("IsMoving", false); 
        }
        else                                 
	    { 
			Anim.SetBool("IsMoving", true);
	    }
		position = RBody.position;
	}
	

	
	void OnCollisionEnter()
	{
		Anim.SetBool("IsGrounded", true);
	}
	
	public void JumpEvent()
	{
		Anim.SetBool("IsGrounded", false);
	}
}
