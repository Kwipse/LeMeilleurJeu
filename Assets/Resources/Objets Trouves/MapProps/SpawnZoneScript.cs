using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZoneScript : MonoBehaviour
{
   /*
   - a qui appartient la zone
   - capture par les heros
   - capture par le roi
   - modifie l'état de la capture
   */
   /*
   necessite un collider en trigger sur la zone
   necessite un rigidbody en kinematic sur les unités
   */
   public bool PossedeparleRoi = false;
   public int niveaudecapture=0 ;
   //  1 les heros capture le point
   // 0 personne ne capture ou les deux équipes sont sur le point
   // -1 le roi capture
   private bool heroscapture ;
   private bool unitcapture ;
  

// et appeler pour chaque collider dans le trigger
    private void OnTriggerStay(Collider other) {
     
        if(other.CompareTag("Player") )
        {
            heroscapture = true ;
        }
       
        else if(other.CompareTag("Unit") )
        {
            unitcapture = true ;
        }
    }

    private void FixedUpdate() {
        // le spawn appartien au roi
        if(PossedeparleRoi && heroscapture && !unitcapture )
        {
            //les heros capture le spawn
            Debug.Log("le spawn est capturé par les heros");
            Debug.Log("unitcapture "+unitcapture);

        }
        //le spawn est possédé par les heros
        if( ! PossedeparleRoi && unitcapture && ! heroscapture)
        {
            //le roi capture le spawn
            Debug.Log("le spawn est capturé par le roi");
        }
        //reset des variables de controle de la capture
        unitcapture = false;
        heroscapture = false;
    }


}
