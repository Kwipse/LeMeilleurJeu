using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace LesMeilleursScripts
{
    [RequireComponent(typeof(RTSResourcesManager))]
    [RequireComponent(typeof(RTSSelection))]
    public class RTSUI : MonoBehaviour
    {
        private int selectionIndex = 0 ;
        public TextMeshProUGUI resourceText,selectionText,ordreText;
        public RTSResourcesManager rrm;
        public RTSSelection rs;

        void Start()
        {
            rrm = gameObject.GetComponent<RTSResourcesManager>();
            rs = gameObject.GetComponent<RTSSelection>();

            RefreshResources();
        }
        void Update()
        {// cet update est vraiment sale
        // penser a call au moment opportun pour eviter le gaspi
            RefreshResources();
            RefreshSubSelection();
        }

        public void RefreshResources()
        {
            resourceText.text = rrm.GoldAmount().ToString(); 
        }

        public void RefreshSubSelection()
        {
            //on affiche le nom de la subselection
            //on met a jour les ordres possibles 
            if(rs.CurrentSelectionLenght()>0)
            {
                selectionText.text = rs.MainSubSelection().ElementAt(0).name ;
            }
            else selectionText.text = "aucune selection";
        }

        public void UpdateCommands()
        {
            
        }
    }
}
