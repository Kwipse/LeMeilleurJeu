public abstract class AIState  
{
    public AIStateController sc;
    public string name;
    public void OnStateEnter(AIStateController stateController)
    {
        name=this.GetType().Name;
        // Code placed here will always run
        sc = stateController;
        OnEnter();
    }

    public virtual void OnEnter()
    {
        // Code placed here can be overridden
    }

    public void OnStateUpdate()
    {
        // Code placed here will always run
        OnUpdate();
    }

    public virtual void OnUpdate()
    {
        // Code placed here can be overridden
    }

    public void OnStateHurt()
    {
        // Code placed here will always run
        OnHurt();
    }

    public virtual void OnHurt()
    {
        // Code placed here can be overridden
    }

    public void OnStateExit()
    {
        // Code placed here will always run
        OnExit();
    }

    public virtual void OnExit()
    {
        // Code placed here can be overridden
    }
} 
