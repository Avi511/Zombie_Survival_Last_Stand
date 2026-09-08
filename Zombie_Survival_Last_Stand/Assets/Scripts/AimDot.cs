using UnityEngine;
using UnityEngine.UI;

public class AimDot : MonoBehaviour
{
    [SerializeField] private Color dotColor = Color.red;
    [SerializeField] private Vector2 dotSize = new Vector2(6f, 6f);

    private void Start()
    {
        CreateAimDot();
    }

    private void CreateAimDot()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("AimCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
        }

        GameObject dotObject = GameObject.Find("AimDot");
        if (dotObject == null)
        {
            dotObject = new GameObject("AimDot");
            dotObject.transform.SetParent(canvas.transform, false);
        }

        RectTransform rectTransform = dotObject.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = dotObject.AddComponent<RectTransform>();
        }

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = dotSize;
        rectTransform.anchoredPosition = Vector2.zero;

        Image image = dotObject.GetComponent<Image>();
        if (image == null)
        {
            image = dotObject.AddComponent<Image>();
        }

        image.color = dotColor;
        image.raycastTarget = false;
    }
}
