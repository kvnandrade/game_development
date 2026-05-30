using System.Collections.Generic;
using UnityEngine;

namespace ValeDosCristais
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] private float framesPerSecond = 8f;

        private readonly Dictionary<string, Sprite[]> states = new Dictionary<string, Sprite[]>();
        private SpriteRenderer spriteRenderer;
        private string currentState;
        private int frameIndex;
        private float frameTimer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(currentState) || !states.ContainsKey(currentState))
            {
                return;
            }

            Sprite[] frames = states[currentState];
            if (frames.Length == 0)
            {
                return;
            }

            if (frames.Length == 1)
            {
                spriteRenderer.sprite = frames[0];
                return;
            }

            frameTimer += Time.deltaTime;
            float frameDuration = 1f / framesPerSecond;
            if (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                frameIndex = (frameIndex + 1) % frames.Length;
                spriteRenderer.sprite = frames[frameIndex];
            }
        }

        public void Define(string state, params string[] spriteNames)
        {
            Sprite[] frames = new Sprite[spriteNames.Length];
            for (int i = 0; i < spriteNames.Length; i++)
            {
                frames[i] = SpriteLibrary.Get(spriteNames[i]);
            }

            states[state] = frames;
            if (string.IsNullOrEmpty(currentState))
            {
                Play(state, true);
            }
        }

        public void Play(string state)
        {
            Play(state, false);
        }

        public void Play(string state, bool force)
        {
            if (!force && currentState == state)
            {
                return;
            }

            if (!states.ContainsKey(state))
            {
                return;
            }

            currentState = state;
            frameIndex = 0;
            frameTimer = 0f;
            Sprite[] frames = states[currentState];
            if (frames.Length > 0)
            {
                spriteRenderer.sprite = frames[0];
            }
        }
    }
}
