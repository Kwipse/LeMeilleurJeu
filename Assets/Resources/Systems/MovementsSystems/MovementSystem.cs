using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scriptablesobjects
{
    public abstract class MovementSystem : ScriptableObject
    {
        public float moveSpeed;
        public bool movingStopsForces;

        [HideInInspector] public GameObject movingGo;
        [HideInInspector] public Transform movingTr;
        [HideInInspector] public Rigidbody movingRb;
        [HideInInspector] public Vector3 moveVect;


        public void SetMovingObject(GameObject go)
        {
            movingGo = go;
            movingRb = go.GetComponent<Rigidbody>();
            movingTr = go.transform;
            moveVect = Vector3.zero;
            //Debug.Log($"MovementSystem set for {go.name}"); 
        }


        //Base methods
        public void AddForce(Vector3 force) { movingRb.AddForce(force); }
        public void AddRelativeForce(Vector3 force) { movingRb.AddRelativeForce(force); }
        public void Move(Vector3 direction) { moveVect += direction; }
        public void Teleport(Vector3 position) { movingTr.position = position; }

        public void UpdatePosition()
        {
            if ((moveVect != Vector3.zero) && (movingStopsForces)) 
            {
                Vector3 projected = Vector3.Project(movingRb.velocity, moveVect);
                movingRb.velocity -= projected;

                //Vector3 counterForce = movingRb.velocity - projected;
                //Debug.Log($"Movement : Velocity = {movingRb.velocity}, MoveVect = {moveVect}, Projected = {projected}, CounterForce = {counterForce}");
                //movingRb.velocity -= counterForce;

            }

            movingTr.position += moveVect * moveSpeed * 0.1f;
            moveVect = Vector3.zero;
        }
    }
}
