using UnityEngine;

[RequireComponent(typeof(MiningAbility))]
[RequireComponent(typeof(AIStateController))]
public class Sbire2 : Unit
{

    //on experimente le minage
    HealthSystem health;
    public AIStateController _aisc; 
    public MiningAbility _ma;

    public override void Awake()
    {
        health = GetComponent<HealthSystem>();
        if( _aisc == null)
        {
            Debug.Log($"heu___yapasd'ia");
        }
        _ma = GetComponent<MiningAbility>();
        base.Awake();
        

    }


public override void AttackAction() 
{}

public void LaunchAnimationByName(string _name)
{
    gameObject.GetComponent<Animator>().Play(_name);
}
}
