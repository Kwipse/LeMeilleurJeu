using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


    public class RTSUnitPanelScript : NetworkBehaviour
    {
        // gere le panneau d'affichage de la selection
        public GameObject icon;

        SelectionSystem SS;


        void Awake() {
            //rTSSelection = transform.root.GetComponent<RTSSelection>();
            SS = transform.root.GetComponent<SelectionSystem>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) {enabled = false;}
        }

        void Start()
        {
            //Subscribe to events
            SS.ObjectSelectedEvent += OnObjectAddedToSelection;
            SS.ObjectUnselectedEvent += OnObjectRemovedFromSelection;
        }

        // Selection Events
        void OnObjectAddedToSelection(GameObject addedObject) {
            AddChildren(1); }

        void OnObjectRemovedFromSelection(GameObject removedObject) {
            DeleteChildren(1); }

        //public void UpdateIconCount()
        //{
        //    int currentChildCount = transform.childCount;
        //    int targetChildCount = SS.GetSelectionCount();

        //    // Add or delete children based on the difference between the current and target counts
        //    if (currentChildCount < targetChildCount)
        //    {
        //        AddChildren(targetChildCount - currentChildCount);
        //    }
        //    else if (currentChildCount > targetChildCount)
        //    {
        //        DeleteChildren(currentChildCount - targetChildCount);
        //    }
        //}


        //On est surs de ces fcts ?
        //
        void AddChildren(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Instantiate(icon, transform);            
            }
        }
        //
        void DeleteChildren(int count)
        {
            for (int i = 0; i < count; i++)
            {
                DeleteLastIcon();            
            }
        }
        //
        void DeleteLastIcon()
        {
            Transform lastChild = transform.GetChild(transform.childCount - 1);
            Destroy(lastChild.gameObject);

        }
    }

