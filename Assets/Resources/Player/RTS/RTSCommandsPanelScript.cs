using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class RTSCommandsPanelScript : MonoBehaviour
    {
        //ce panneau gere les commandes des unités/batiments
        public GameObject icon;
        public RTSSelection rTSSelection;

        void Awake() {
            rTSSelection = transform.root.GetComponent<RTSSelection>();
        }

        public void UpdateIconCount()
        {
            int currentChildCount = transform.childCount;
            //changer le compte en fct de l'unité
            int targetChildCount =rTSSelection.CurrentSelectionLenght();

            // Add or delete children based on the difference between the current and target counts
            if (currentChildCount < targetChildCount)
            {
                AddChildren(targetChildCount - currentChildCount);
            }
            else if (currentChildCount > targetChildCount)
            {
                DeleteChildren(currentChildCount - targetChildCount);
            }
        }

        void AddChildren(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(icon, transform);            
            }
        }
        void DeleteChildren(int count)
        {
            for (int i = 0; i < count; i++)
            {
                DeleteLastIcon();            
            }
        }
        

        void DeleteLastIcon()
        {
            Transform lastChild = transform.GetChild(transform.childCount - 1);
            Destroy(lastChild.gameObject);
            
        }
    }

