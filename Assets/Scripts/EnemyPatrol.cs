using UnityEngine;

namespace ValeDosCristais
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class EnemyPatrol : MonoBehaviour
    {
        [SerializeField] private float speed = 1.8f;
        [SerializeField] private float patrolDistance = 3f;

        private float leftLimit;
        private float rightLimit;
        private int direction = 1;
        private SpriteRenderer spriteRenderer;
        private SpriteAnimator animator;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<SpriteAnimator>();
        }

        private void Start()
        {
            leftLimit = transform.position.x - patrolDistance;
            rightLimit = transform.position.x + patrolDistance;
            if (animator != null)
            {
                animator.Define("walk", "enemy_walk_0", "enemy_walk_1");
                animator.Play("walk");
            }
        }

        private void Update()
        {
            transform.Translate(Vector2.right * (direction * speed * Time.deltaTime));
            if (transform.position.x >= rightLimit)
            {
                direction = -1;
            }
            else if (transform.position.x <= leftLimit)
            {
                direction = 1;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction < 0;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(transform.position);
            }
        }
    }
}
