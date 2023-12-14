using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FPSJump : NetworkBehaviour
{
    public int nbJumpMax = 1;
    public float jumpForce = 500f;

    Rigidbody RBody;
	
    //Declarations jump basique
    bool jumpKey = false;
    int nbJump = 0;

    void Start()
    {
        RBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) jumpKey = true;
    }

    void FixedUpdate()
    {
        if (jumpKey && (nbJumpMax > nbJump ))
        {
            RBody.AddForce(new Vector3(0, jumpForce, 0));
            jumpKey = false;
            nbJump += 1;
        }
    }


    void OnCollisionEnter(Collision col) {
        if (col.gameObject.tag == "ground") {
            nbJump = 0;
            jumpKey = false; } }

}

