using UnityEngine;

[RequireComponent(typeof(MiningAbility))]
[RequireComponent(typeof(AIStateController))]
public class Sbire2 : Unit
{

    //on experimente le minage
    HealthSystem health;
    public AIStateController _aisc; 
    public MiningAbility _ma;
    public Animator _anim;

    public override void Awake()
    {
        health = GetComponent<HealthSystem>();
        if( _aisc == null)
        {
            Debug.Log($"heu___yapasd'ia");
        }
        _ma = GetComponent<MiningAbility>();
        _anim = gameObject.GetComponent<Animator>();
        base.Awake();
       

    }


public override void AttackAction() 
{}

public void LaunchAnimationByName(string _name)
{
  _anim.Play(_name);
}

public void SetupNewStateForAnimation(string name,AnimationClip clip) 
{
}

}
