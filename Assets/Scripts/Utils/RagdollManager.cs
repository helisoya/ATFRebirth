using UnityEngine;

/// <summary>
/// Handles a ragdoll
/// </summary>
public class RagdollManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Rigidbody[] rbs;
    [SerializeField] private Collider[] cols;

    void Start()
    {
        StopRagdoll();
    }

    /// <summary>
    /// Activates the ragdoll for a few seconds
    /// </summary>
    public void ActivateRagdoll()
    {
        print("Activating Ragdoll");
        animator.enabled = false;
        playerRb.isKinematic = true;
        playerCollider.enabled = true;
        EnableRigibodies(true);
        EnableColliders(true);
        Invoke("StopRagdoll", 5);
    }

    /// <summary>
    /// Disables the ragdoll
    /// </summary>
    public void StopRagdoll()
    {
        EnableRigibodies(false);
        EnableColliders(false);
    }

    /// <summary>
    /// Enables the ragdoll's colliders
    /// </summary>
    /// <param name="value">Are the collider enabled ?</param>
    void EnableColliders(bool value)
    {
        foreach (Collider col in cols)
        {
            col.enabled = value;
        }
    }

    /// <summary>
    /// Enables the ragdoll's rigidbodies
    /// </summary>
    /// <param name="value">Are the rigidbody enabled ?</param>
    void EnableRigibodies(bool value)
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = !value;
        }
    }
}
