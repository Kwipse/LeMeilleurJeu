using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace classes 
{
    public class Ressource
    {
        /*
        Cette classe permet de définir un container de ressource, 
        avec une quantité "res", limitée entre "min" et "max".
        */

        int min, max, res;
        int lastRessource;

        public Ressource(int Min, int Max, int InitialRes)
        {
            min = Min;
            max = Max;
            res = InitialRes;
            lastRessource = InitialRes;
        }


        //Event on res amount changed
        public delegate void RessourceEvent();
        public delegate void RessourceChangedEvent(int newResAmount);
        public event RessourceChangedEvent ChangeEvent;
        public event RessourceEvent HitMinEvent;
        public event RessourceEvent HitMaxEvent;



        //Manage min/max
        public void SetRessourceMin(int newMin) { min = newMin; }
        public void SetRessourceMax(int newMax) { max = newMax; }


        //Ressource Infos
        public int GetRessource() { return res; }
        public int GetRessourceMin() { return min; }
        public int GetRessourceMax() { return max; }
        public bool TestForRessource(int testRes) { return (testRes <= res); }


        // Manage res
        public void AddRessource(int amount) {
            res = ClampRessource(res + amount);
            sendEvents(); }

        public void RemoveRessource(int amount) {
            res = ClampRessource(res - amount);
            sendEvents(); }

        public void SetRessourceTo(int toSetAmount) {
            res = ClampRessource(toSetAmount);
            sendEvents(); }

        public void SetRessourceToMin() {
            res = min; 
            sendEvents(); }

        public void SetRessourceToMax() { 
            res = max; 
            sendEvents(); }


        //Event sender
        void sendEvents()
        {
            //Send change event
            if (ChangeEvent != null) { ChangeEvent(res); }

            //Send min event
            if ((lastRessource != min) && (res == min)) {
                if (HitMinEvent != null) { HitMinEvent(); } }

            //Send max event
            if ((lastRessource != max) && (res == max)) {
                if (HitMaxEvent != null) { HitMaxEvent(); } }

            lastRessource = res;
        }

        
        int ClampRessource(int newRes) {
            return (newRes < min) ? min : (newRes > max) ? max : newRes; }
    }
}
