using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 1

namespace LesMeilleursScripts
{
    public class testRtsScript : MonoBehaviour
    {
        // travail sur l'ui rts
         //IPointerClickHandler // 2
        public float scrollSpeed = 5f;
        public ScrollRect scrollRect;
        private bool isMouseOverPanel;
        
    
        void Update()
        {
            if (isMouseOverPanel)
            {                   
                Debug.Log("over panel");

                float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                ScrollPanelByMouse(scrollInput);
            }
        }
        void ScrollPanelByMouse(float scrollInput)
        {
            if (scrollRect != null)
            {
                Debug.Log("scrolled ui");
                // Adjust the scroll speed and direction based on your needs
                float scrollAmount = scrollInput * scrollSpeed;
                Vector2 scrollDelta = new Vector2(0, scrollAmount);

                scrollRect.normalizedPosition += scrollDelta;
                scrollRect.normalizedPosition = new Vector2(
                    Mathf.Clamp01(scrollRect.normalizedPosition.x),
                    Mathf.Clamp01(scrollRect.normalizedPosition.y)
                );
            }
        }

        void OnMouseEnter()
        {
            Debug.Log("over panel");

            isMouseOverPanel = true;
        }

        void OnMouseExit()
        {
            Debug.Log(" not over panel");

            isMouseOverPanel = false;
        }
        /*
        public void OnPointerClick(PointerEventData eventData) // 3
        {
            print("I was clicked");
            target = Color.blue;
        }*/
        
    }
}
