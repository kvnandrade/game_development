using UnityEngine;

namespace ValeDosCristais
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class GoalPortal : MonoBehaviour
    {
        private GameManager manager;
        private SpriteAnimator animator;

        public void Configure(GameManager gameManager)
        {
            manager = gameManager;
        }

        private void Awake()
        {
            animator = GetComponent<SpriteAnimator>();
        }

        private void Start()
        {
            if (animator != null)
            {
                animator.Define("idle", "goal_0", "goal_1");
                animator.Play("idle");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && manager != null)
            {
                manager.CompleteLevel();
            }
        }
    }
}
