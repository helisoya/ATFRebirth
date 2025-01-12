using Mirror;
using UnityEngine;

/// <summary>
/// Represents bigfoot's logic
/// </summary>
public class BigfootLogic : EnnemyLogic
{

    [Header("Bigfoot")]
    [SerializeField] private float sightRange;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected AudioSource audioSource;
    private float lastAttack;
    private PlayerNetwork currentTarget;
    private bool hasAquiredTargetAlready;

    protected override void Logic()
    {
        float minDist = Mathf.Infinity;
        PlayerNetwork selectedPlayer = null;
        float computedDist;

        // Find most direct player
        foreach (PlayerNetwork player in GameManager.instance.players)
        {
            if (!player.health.Alive) continue;

            computedDist = Vector3.Distance(transform.position, player.transform.position);
            if (computedDist < minDist)
            {
                selectedPlayer = player;
                minDist = computedDist;
            }
        }

        RaycastHit hit;
        bool hasLineOfSight = false;
        if (selectedPlayer && Physics.Raycast(transform.position, ((selectedPlayer.transform.position + Vector3.up) - transform.position).normalized, out hit, sightRange))
        {
            hasLineOfSight = hit.transform.gameObject.layer == 8;
        }

        // Target if possible
        if (selectedPlayer && (hasAquiredTargetAlready || (minDist <= sightRange && hasLineOfSight)))
        {
            hasAquiredTargetAlready = true;
            currentTarget = selectedPlayer;
        }

        if (!currentTarget)
        {
            agent.isStopped = true;
            animator.SetBool("Move", false);
            return;
        }

        // Do if has a target

        transform.LookAt(currentTarget.transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        agent.SetDestination(currentTarget.transform.position);

        if ((agent.isStopped && (minDist > attackRange || !hasLineOfSight)) ||
            (!agent.isStopped && minDist <= attackRange && hasLineOfSight))
        {
            agent.isStopped = !agent.isStopped;
        }

        animator.SetBool("Move", !agent.isStopped);

        if (hasLineOfSight && minDist <= attackRange && Time.time - lastAttack >= attackCooldown)
        {
            lastAttack = Time.time;
            currentTarget.health.TakeDamage(baseDamage);
            RpcPlaySFX();
        }

    }

    protected override void Setup()
    {
        base.Setup();
        hasAquiredTargetAlready = false;
    }

    public override void OnAggression()
    {
        hasAquiredTargetAlready = true;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }


    /// <summary>
    /// Plays the attack SFX (Clients)
    /// </summary>
    [ClientRpc]
    void RpcPlaySFX()
    {
        audioSource.Stop();
        audioSource.Play();
        animator.BaseAnimator.SetTrigger("Kick");
    }
}
