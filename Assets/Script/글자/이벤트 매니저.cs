using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(SkinController))]
public class 이벤트매니저 : MonoBehaviour
{
    SkinController skinController;
    [SerializeField] private RectTransform effectCanvas;
    [SerializeField] private Sprite heartSprite;
    [SerializeField, Min(1)] private int heartCount = 6;
    [SerializeField, Min(0.1f)] private float heartLifetime = 1.6f;
    [SerializeField, Min(0f)] private float heartRise = 90f;
    [SerializeField, Min(0f)] private float heartSpread = 55f;

    void Awake()
    {
        skinController = GetComponent<SkinController>();
    }
    // Update is called once per frame

    public void StartEvent(List<string> type)
    {
        if (type == null)
            return;

        foreach (string t in type)
        {
            switch (t?.Trim().ToLowerInvariant())
            {
                case "attack":
                    skinController.Attack();
                    break;
                case "jump":
                    skinController.Jump();
                    break;
                case "heart":
                    SpawnHearts();
                    break;
            }
        }
    }

    private void SpawnHearts()
    {
        if (effectCanvas == null || heartSprite == null)
        {
            Debug.LogWarning("Heart effect needs a Canvas and a heart sprite.", this);
            return;
        }

        // The avatar position is measured from the center of this Canvas.
        Vector2 center = skinController.position + new Vector2(0f, 45f * skinController.size);
        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = new GameObject("Heart Effect", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = (RectTransform)heart.transform;
            rect.SetParent(effectCanvas, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(25f, 25f);
            Vector2 start = center + new Vector2(Random.Range(-heartSpread, heartSpread), Random.Range(-20f, 20f));
            rect.anchoredPosition = start;

            Image image = heart.GetComponent<Image>();
            image.sprite = heartSprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            StartCoroutine(AnimateHeart(rect, image, start, Random.Range(0.8f, 1.2f)));
        }
    }

    private IEnumerator AnimateHeart(RectTransform rect, Image image, Vector2 start, float speed)
    {
        float duration = heartLifetime / speed;
        float elapsed = 0f;
        float drift = Random.Range(-20f, 20f);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            rect.anchoredPosition = start + new Vector2(drift * progress, heartRise * progress);
            Color color = image.color;
            color.a = 1f - Mathf.SmoothStep(0f, 1f, progress);
            image.color = color;
            yield return null;
        }
        Destroy(rect.gameObject);
    }
}
