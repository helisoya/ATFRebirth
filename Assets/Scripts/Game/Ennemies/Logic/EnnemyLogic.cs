using Mirror;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.AI;

public class EnnemyLogic : NetworkBehaviour
{
    [Header("General AI")]
    [SerializeField] protected float baseSpeed;
    [SerializeField] protected int baseDamage;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected NetworkedAnimator animator;

    public bool Activated { get; private set; } = true;

    void Start()
    {
        if (isServer)
        {
            agent.speed = baseSpeed;
            Setup();
        }
    }

    /// <summary>
    /// Desactivate the AI
    /// </summary>
    public void Desactivate()
    {
        Activated = false;
        agent.isStopped = true;
    }

    void Update()
    {
        if (!Activated || !isServer) return;
        Logic();
    }

    /// <summary>
    /// Represents the ennemy's logic
    /// </summary>
    protected virtual void Logic()
    {

    }

    /// <summary>
    /// Represents the ennemy's setup
    /// </summary>
    protected virtual void Setup()
    {

    }

    /// <summary>
    /// Callback for when the ennemy is being attacked
    /// </summary>
    public virtual void OnAggression()
    {

    }
}
