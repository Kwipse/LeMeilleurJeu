using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class FPSMovement : MonoBehaviour
{
    public float moveSpeed;
    public int nbJumpMax = 1;
    public float jumpForce = 500f;

    Rigidbody RBody;
	Vector3 translation;
    Vector3 velocity;
    bool jumpKey = false;
    int nbJump = 0;

    void Awake()
    {
        RBody = GetComponent<Rigidbody>();
    }

	void Update()
	{
		KeyboardInputs();
	}

	void KeyboardInputs()
	{
        translation = Vector3.zero;

        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) 
            translation += transform.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) 
            translation -= transform.forward;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
            translation += transform.right;
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) 
            translation -= transform.right;

        if (Input.GetKeyDown(KeyCode.Space)) 
            jumpKey = true;
    }


    void FixedUpdate()
    {
        MoveFPSPlayer();
    }
	
	void MoveFPSPlayer()
    {
        transform.position += translation * moveSpeed;

        if (jumpKey && (nbJumpMax > nbJump )) {
            RBody.AddForce(new Vector3(0, jumpForce, 0));
            nbJump += 1; } 

        jumpKey = false;
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "ground") {
            //jumpKey = false;
            nbJump = 0; } }

}
