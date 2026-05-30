using System.Collections;
using UnityEngine;

namespace ValeDosCristais
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteAnimator))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 6.7f;
        [SerializeField] private float jumpForce = 9.2f;
        [SerializeField] private float climbSpeed = 4.1f;

        private readonly RaycastHit2D[] groundHits = new RaycastHit2D[6];
        private Rigidbody2D body;
        private BoxCollider2D boxCollider;
        private SpriteRenderer spriteRenderer;
        private SpriteAnimator animator;
        private ContactFilter2D groundFilter;
        private Vector2 spawnPoint;
        private int ladderContacts;
        private bool jumpPressed;
        private bool grounded;
        private bool climbing;
        private bool invulnerable;
        private float defaultGravity;
        private float horizontalInput;
        private float verticalInput;

        public GameManager Manager { get; private set; }
        public AudioManager Audio { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<SpriteAnimator>();

            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            defaultGravity = body.gravityScale;

            groundFilter = new ContactFilter2D();
            groundFilter.useTriggers = false;
            groundFilter.SetLayerMask(Physics2D.DefaultRaycastLayers);

            animator.Define("idle", "player_idle_0", "player_idle_1");
            animator.Define("walk", "player_walk_0", "player_walk_1");
            animator.Define("run", "player_run_0", "player_run_1");
            animator.Define("jump", "player_jump_0");
            animator.Define("climb", "player_climb_0", "player_climb_1");
            animator.Define("hurt", "player_hurt_0");
        }

        private void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
            {
                jumpPressed = true;
            }

            if (transform.position.y < -6f)
            {
                TakeDamage(transform.position + Vector3.down);
            }
        }

        private void FixedUpdate()
        {
            UpdateGrounded();

            bool wantsToClimb = ladderContacts > 0 && Mathf.Abs(verticalInput) > 0.05f;
            if (wantsToClimb)
            {
                climbing = true;
            }
            else if (ladderContacts == 0 || jumpPressed)
            {
                climbing = false;
            }

            float speed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? runSpeed : walkSpeed;

            if (climbing)
            {
                body.gravityScale = 0f;
                body.velocity = new Vector2(horizontalInput * walkSpeed, verticalInput * climbSpeed);
            }
            else
            {
                body.gravityScale = defaultGravity;
                body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            }

            if (jumpPressed && grounded && !climbing)
            {
                body.velocity = new Vector2(body.velocity.x, jumpForce);
                grounded = false;
                if (Audio != null)
                {
                    Audio.Play("jump");
                }
            }

            jumpPressed = false;
            UpdateAnimation(speed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Ladder>() != null)
            {
                ladderContacts++;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<Ladder>() != null)
            {
                ladderContacts = Mathf.Max(0, ladderContacts - 1);
                if (ladderContacts == 0)
                {
                    climbing = false;
                }
            }
        }

        public void Configure(GameManager manager, AudioManager audioManager)
        {
            Manager = manager;
            Audio = audioManager;
        }

        public void SetSpawn(Vector2 point)
        {
            spawnPoint = point;
            Respawn();
        }

        public void Respawn()
        {
            transform.position = spawnPoint;
            body.velocity = Vector2.zero;
            climbing = false;
            ladderContacts = 0;
            body.gravityScale = defaultGravity;
        }

        public void TakeDamage(Vector2 source)
        {
            if (invulnerable || Manager == null)
            {
                return;
            }

            StartCoroutine(DamageRoutine(source));
            Manager.PlayerDamaged(this);
        }

        private void UpdateGrounded()
        {
            int hitCount = boxCollider.Cast(Vector2.down, groundFilter, groundHits, 0.08f);
            grounded = hitCount > 0;
        }

        private void UpdateAnimation(float currentSpeed)
        {
            if (invulnerable)
            {
                animator.Play("hurt");
            }
            else if (climbing)
            {
                animator.Play("climb");
            }
            else if (!grounded)
            {
                animator.Play("jump");
            }
            else if (Mathf.Abs(horizontalInput) > 0.05f)
            {
                animator.Play(currentSpeed >= runSpeed - 0.1f ? "run" : "walk");
            }
            else
            {
                animator.Play("idle");
            }

            if (Mathf.Abs(horizontalInput) > 0.05f)
            {
                spriteRenderer.flipX = horizontalInput < 0f;
            }
        }

        private IEnumerator DamageRoutine(Vector2 source)
        {
            invulnerable = true;
            climbing = false;
            body.gravityScale = defaultGravity;
            float direction = transform.position.x >= source.x ? 1f : -1f;
            body.velocity = new Vector2(direction * 5.2f, 6f);

            float elapsed = 0f;
            while (elapsed < 1.3f)
            {
                spriteRenderer.color = new Color(1f, 0.45f, 0.45f, 1f);
                yield return new WaitForSeconds(0.08f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.08f);
                elapsed += 0.16f;
            }

            invulnerable = false;
            spriteRenderer.color = Color.white;
        }
    }
}
