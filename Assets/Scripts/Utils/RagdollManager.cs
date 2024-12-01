using UnityEngine;

/// <summary>
/// Handles a ragdoll
/// </summary>
public class RagdollManager : MonoBehaviour
{
    [Header("Non-Rigidbody")]
    [SerializeField] private Animator[] animators;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private Collider[] playerColliders;

    [Header("Rigidbody")]
    [SerializeField] private Rigidbody[] rbs;
    [SerializeField] private Collider[] cols;

    void Awake()
    {
        StopRagdoll();
    }

    /// <summary>
    /// Activates the ragdoll for a few seconds
    /// </summary>
    public void ActivateRagdoll()
    {
        if (playerRb) playerRb.isKinematic = true;
        if (playerColliders.Length > 0) EnableColliders(playerColliders, false);
        EnableAnimators(false);
        EnableRigibodies(true);
        EnableColliders(cols, true);
        Invoke("StopRagdoll", 5);
    }

    /// <summary>
    /// Disables the ragdoll
    /// </summary>
    public void StopRagdoll()
    {
        EnableRigibodies(false);
        EnableColliders(cols, false);
    }

    /// <summary>
    /// Enables the ragdoll's colliders
    /// </summary>
    /// <param name="value">Are the collider enabled ?</param>
    void EnableColliders(Collider[] cols, bool value)
    {
        foreach (Collider col in cols)
        {
            col.enabled = value;
        }
    }

    /// <summary>
    /// Enables the ragdoll's colliders
    /// </summary>
    /// <param name="value">Are the collider enabled ?</param>
    void EnableAnimators(bool value)
    {
        foreach (Animator animator in animators)
        {
            animator.enabled = value;
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
