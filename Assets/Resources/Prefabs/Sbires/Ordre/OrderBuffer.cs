using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class OrdreBuffer : MonoBehaviour
{
    //sur une unité
    //permet de stocker une pile d'ordre
    //si l'unité n'as pas d'ordre en cours, envoil'ordre
    //ordre de déplacement
    //ordre de skill
    //
    
    Unit _unit;
    public List<OrderRTS> availableOrder;
    public Queue<OrderRTS> _queue;
    public void Awake()
    {
        _unit = gameObject.GetComponent<Unit>();
        availableOrder = new List<OrderRTS>() ;
        _queue = new Queue<OrderRTS>();

    }

    



}

