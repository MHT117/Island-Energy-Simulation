using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraPanZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    [Tooltip("Scroll wheel zoom speed")]
    public float zoomSpeed = 2f;
    [Tooltip("Min orthographic size")]
    public float minZoom = 3f;
    [Tooltip("Max orthographic size")]
    public float maxZoom = 15f;

    [Header("Pan Settings")]
    [Tooltip("Edge‐of‐screen pan speed (units/sec)")]
    public float panSpeed = 5f;
    [Tooltip("Thickness in pixels of the screen edge for panning")]
    public float screenEdgeThickness = 10f;

    [Header("World Bounds")]
    [Tooltip("Leftmost world X coordinate")]
    public float minX;
    [Tooltip("Rightmost world X coordinate")]
    public float maxX;
    [Tooltip("Bottom world Y coordinate")]
    public float minY;
    [Tooltip("Top world Y coordinate")]
    public float maxY;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam.orthographic)
            Debug.LogWarning("CameraPanZoom expects an orthographic camera!");
    }

    void Update()
    {
        // ─── ZOOM ────────────────────────────────────────────────────────
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - scroll * zoomSpeed,
                minZoom, maxZoom
            );
        }

        // ─── PAN (screen‐edge) ────────────────────────────────────────────
        Vector3 pos = transform.position;
        Vector2 mouse = Input.mousePosition;

        if (mouse.x <= screenEdgeThickness)
            pos.x -= panSpeed * Time.deltaTime;
        else if (mouse.x >= Screen.width - screenEdgeThickness)
            pos.x += panSpeed * Time.deltaTime;

        if (mouse.y <= screenEdgeThickness)
            pos.y -= panSpeed * Time.deltaTime;
        else if (mouse.y >= Screen.height - screenEdgeThickness)
            pos.y += panSpeed * Time.deltaTime;

        // ─── CLAMP TO WORLD BOUNDS ───────────────────────────────────────
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}
