using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace scriptablesobjects 
{
    public abstract class UI : ScriptableObject 
    {
        public GameObject UIPrefab;
        [HideInInspector] public GameObject UIObject;

        GameObject UIOwner;
        List<TMP_Text> UITexts;
        bool isUISet = false;


        public void SetUI(GameObject UIOwner)
        {
            UIObject = Instantiate(UIPrefab);
            UIObject.transform.SetParent(UIOwner.transform);
            InitUITexts();

            //Debug.Log($"UI : Owned by {UIOwner.name}");
            isUISet = true;
            OnSetUI(UIOwner);
        }

        void InitUITexts()
        {
            UITexts = new List<TMP_Text>();
            foreach(var text in UIObject.GetComponentsInChildren<TMP_Text>())
                UITexts.Add(text);
        }

        public void SetUIText(string UITextName, string newText)
        {
            if (!isUISet) { return; }

            foreach (var text in UITexts) {
                if (text.name == UITextName)  {
                    text.text = newText;
                    return; } }

            //Debug.Log($"No UI text of name {UITextName}");
        }

        public GameObject GetUIOwner() { return UIObject.transform.root.gameObject; }

        public abstract void OnSetUI(GameObject UIOwner);
    }

    

}
