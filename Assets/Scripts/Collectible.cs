using UnityEngine;

namespace ValeDosCristais
{
    public enum CollectibleType
    {
        Coin,
        Gem,
        Life
    }

    [RequireComponent(typeof(Collider2D))]
    public sealed class Collectible : MonoBehaviour
    {
        public CollectibleType Type { get; private set; }
        public int ScoreValue { get; private set; }
        public GameManager Manager { get; private set; }

        public void Configure(GameManager manager, CollectibleType type, int scoreValue)
        {
            Manager = manager;
            Type = type;
            ScoreValue = scoreValue;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null || Manager == null)
            {
                return;
            }

            Manager.Collect(this);
            gameObject.SetActive(false);
            Destroy(gameObject, 0.05f);
        }
    }
}
