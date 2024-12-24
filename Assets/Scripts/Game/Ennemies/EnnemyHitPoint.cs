using UnityEngine;

/// <summary>
/// Represents a hit point for an ennemy
/// </summary>
public class EnnemyHitPoint : MonoBehaviour
{
    [Header("Hit Point")]
    [SerializeField] private Ennemy linkedEnnemy;
    [SerializeField] private float multiplier;

    void OnTakeHit(int dmg)
    {
        linkedEnnemy.TakeDamage((int)(dmg * multiplier));
    }
}
