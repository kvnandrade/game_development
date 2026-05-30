using System.Collections.Generic;
using UnityEngine;

namespace ValeDosCristais
{
    public static class SpriteLibrary
    {
        private const float PixelsPerUnit = 16f;
        private static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        public static Sprite Get(string name)
        {
            Sprite sprite;
            if (cache.TryGetValue(name, out sprite))
            {
                return sprite;
            }

            Texture2D texture = Resources.Load<Texture2D>("Sprites/" + name);
            if (texture == null)
            {
                texture = CreateFallbackTexture(name);
            }

            texture.filterMode = FilterMode.Point;
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
            cache[name] = sprite;
            return sprite;
        }

        private static Texture2D CreateFallbackTexture(string key)
        {
            Texture2D texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color color = Color.magenta;
            if (key.Contains("ground")) color = new Color(0.39f, 0.25f, 0.16f);
            if (key.Contains("player")) color = new Color(0.2f, 0.75f, 1f);
            if (key.Contains("enemy")) color = new Color(1f, 0.35f, 0.35f);

            Color[] pixels = new Color[16 * 16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
