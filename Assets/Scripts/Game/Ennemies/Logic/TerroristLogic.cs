using Mirror;
using UnityEngine;

public class TerroristLogic : EnnemyLogic
{
    [Header("Terrorist")]
    [SerializeField] protected TPSWeapon weapon;
    [SerializeField] protected float fireRange;
    [SerializeField] protected float fireSpeed;
    [SerializeField] protected AudioSource audioSource;
    private float lastFire;
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
        if (selectedPlayer && Physics.Raycast(transform.position, ((selectedPlayer.transform.position + Vector3.up) - transform.position).normalized, out hit, fireRange))
        {
            hasLineOfSight = hit.transform.gameObject.layer == 8;
        }

        // Target if possible
        if (selectedPlayer && (hasAquiredTargetAlready || (minDist <= fireRange && hasLineOfSight)))
        {
            hasAquiredTargetAlready = true;
            currentTarget = selectedPlayer;
        }

        if (!currentTarget)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0);
            return;
        }

        // Do if has a target

        transform.LookAt(currentTarget.transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        agent.SetDestination(currentTarget.transform.position);

        if ((agent.isStopped && (minDist > fireRange || !hasLineOfSight)) ||
            (!agent.isStopped && minDist <= fireRange && hasLineOfSight))
        {
            agent.isStopped = !agent.isStopped;
        }

        animator.SetFloat("Speed", agent.isStopped ? 0 : 1);

        if (hasLineOfSight && minDist <= fireRange && Time.time - lastFire >= fireSpeed)
        {
            lastFire = Time.time;
            if (Random.Range(0, 10) <= 7)
            {
                currentTarget.health.TakeDamage(baseDamage);
            }
            RpcFireFlare();
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
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }


    /// <summary>
    /// Activates the muzzle flare on the the clients
    /// </summary>
    [ClientRpc]
    void RpcFireFlare()
    {
        audioSource.Stop();
        audioSource.Play();
        weapon.ActivateMuzzleFlare();
    }
}
