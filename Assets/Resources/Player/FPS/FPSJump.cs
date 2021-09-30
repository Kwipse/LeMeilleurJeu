using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSJump : MonoBehaviour
{
    //Declarations physique
    Rigidbody RBody;
    Vector3 velocity;

    //Declaration Animation
    Animator anim;

    //Declarations jump basique
    float jumpForce;
    bool jumpKey;
    

    //Declarations multijump
    int nbJumpMax;
    int nbJump;


    
    


    // Start is called before the first frame update
    void Start()
    {
        //Init Misc
        RBody = GetComponent<Rigidbody>(); //Get Rigidbody
        anim = GetComponent<Animator>(); //Get Animator Controller

        //Init Basic Jump
        jumpForce = 5f;
        jumpKey = false;


        //Init MultiJump
        nbJumpMax = 3;
        nbJump = 0;
    }


    void Update()
    {
        //Intention de saut
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKey = true;
        }
    }

    void FixedUpdate() // FixedUpdate pour la physique du rigidbody
    {
        if (jumpKey && (nbJumpMax > nbJump ))
        {
            //Saut
            velocity = RBody.velocity;
            velocity.y = jumpForce;
            RBody.velocity = velocity;
            jumpKey = false;
            anim.SetBool("IsGrounded", false);

            //Multisaut
            nbJump += 1;
        }
    }

    void OnCollisionEnter()
    {
        //Reset le saut
        nbJump = 0;
        jumpKey = false;
        anim.SetBool("IsGrounded", true);
    }


}

