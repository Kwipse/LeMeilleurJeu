using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldRessourceTextScript : MonoBehaviour
{
    Text ressourceText;
    // Start is called before the first frame update
    void Start()
    {
        ressourceText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //update text
        ressourceText.text = "gold: ";
    }
}
