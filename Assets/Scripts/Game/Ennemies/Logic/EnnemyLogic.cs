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

    public bool Activated { get; set; } = true;

    void Start()
    {
        if (isServer)
        {
            agent.speed = baseSpeed;
            Setup();
        }
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
}
