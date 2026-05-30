using UnityEngine;

namespace ValeDosCristais
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Hazard : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            Damage(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            Damage(other);
        }

        private void Damage(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(transform.position);
            }
        }
    }
}
