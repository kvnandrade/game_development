using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ValeDosCristais
{
    public sealed class HudController : MonoBehaviour
    {
        private Text livesText;
        private Text scoreText;
        private Text collectiblesText;
        private Text levelText;
        private Text messageText;
        private Coroutine messageRoutine;

        public static HudController Create(Transform parent)
        {
            GameObject canvasObject = new GameObject("HUD");
            canvasObject.transform.SetParent(parent, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            canvasObject.AddComponent<GraphicRaycaster>();

            HudController hud = canvasObject.AddComponent<HudController>();
            hud.Build(canvasObject.transform);
            return hud;
        }

        public void SetStats(int lives, int score, int collected, int total, string levelName)
        {
            livesText.text = "Vidas: " + lives;
            scoreText.text = "Pontuacao: " + score;
            collectiblesText.text = "Coletaveis: " + collected + "/" + total;
            levelText.text = levelName;
        }

        public void ShowMessage(string message)
        {
            if (messageRoutine != null)
            {
                StopCoroutine(messageRoutine);
            }

            messageRoutine = StartCoroutine(MessageRoutine(message));
        }

        private void Build(Transform root)
        {
            livesText = CreateText(root, "Lives", new Vector2(20f, -20f), TextAnchor.UpperLeft, 24, Color.white);
            scoreText = CreateText(root, "Score", new Vector2(20f, -52f), TextAnchor.UpperLeft, 24, Color.white);
            collectiblesText = CreateText(root, "Collectibles", new Vector2(20f, -84f), TextAnchor.UpperLeft, 24, Color.white);
            levelText = CreateText(root, "Level", new Vector2(-20f, -20f), TextAnchor.UpperRight, 24, new Color(0.75f, 0.95f, 1f));
            messageText = CreateText(root, "Message", new Vector2(0f, -112f), TextAnchor.UpperCenter, 26, new Color(1f, 0.9f, 0.35f));
            messageText.text = string.Empty;
        }

        private Text CreateText(Transform root, string name, Vector2 anchoredPosition, TextAnchor alignment, int size, Color color)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(root, false);
            Text text = textObject.AddComponent<Text>();
            text.font = GetDefaultFont();
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            RectTransform rect = text.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(580f, 44f);
            if (alignment == TextAnchor.UpperRight)
            {
                rect.anchorMin = new Vector2(1f, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(1f, 1f);
            }
            else if (alignment == TextAnchor.UpperCenter)
            {
                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
            }
            else
            {
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
            }

            rect.anchoredPosition = anchoredPosition;
            return text;
        }

        private Font GetDefaultFont()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
        }

        private IEnumerator MessageRoutine(string message)
        {
            messageText.text = message;
            yield return new WaitForSeconds(2.8f);
            messageText.text = string.Empty;
            messageRoutine = null;
        }
    }
}
