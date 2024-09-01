using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class OrderBuffer : MonoBehaviour
{
    //sur une unité
    //permet de stocker une pile d'ordre
    //si l'unité n'as pas d'ordre en cours, envoil'ordre
    //ordre de déplacement
    //ordre de skill
    //
    
    Unit _unit;
    Queue<OrderRTS> _queue;
    public List<OrderRTS> availableOrder;

    public void Awake()
    {

        _unit = gameObject.GetComponent<Unit>();
        availableOrder = new List<OrderRTS>(_unit.GetAvailableOrder());  
        _queue = new Queue<OrderRTS>();
    }

    public void GiveOrder(OrderRTS ordre) 
    {
        //appeler par le selection system
        //envoi un ordre du joueur directement à l'unité
        //reset la queue des ordres
       // _unit. 

    }
    public void QueueOrder(OrderRTS ordre)
    {
        //met l'ordre sur la queue
        _queue.Enqueue(ordre);
    }
    public OrderRTS GiveNextOrder()
    {
        //envoi à l'unitScript lorsque l'ordre precedent est terminé
        return _queue.Dequeue();
    }
    public  void ResetQueue()
    {
        _queue = new Queue<OrderRTS>();
    }



}

