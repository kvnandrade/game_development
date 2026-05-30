using UnityEngine;

namespace ValeDosCristais
{
    public sealed class FadingParticle : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Vector2 velocity;
        private float life;
        private float maxLife;

        public void Initialize(Color color, Vector2 startVelocity, float duration)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            velocity = startVelocity;
            life = duration;
            maxLife = duration;
        }

        private void Update()
        {
            transform.position += (Vector3)(velocity * Time.deltaTime);
            velocity += Vector2.down * (2.5f * Time.deltaTime);
            life -= Time.deltaTime;

            if (spriteRenderer != null && maxLife > 0f)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Clamp01(life / maxLife);
                spriteRenderer.color = color;
            }

            if (life <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
