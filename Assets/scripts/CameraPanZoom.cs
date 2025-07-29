using UnityEngine;
//using static UnityEditor.PlayerSettings;

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
           // Debug.LogWarning("CameraPanZoom expects an orthographic camera!");

        // ─── START AT MAX ZOOM ──────────────────────────────────────────
        cam.orthographicSize = maxZoom;
    }


    void Update()
    {
        if (GameManager.I.tutorialActive)
            return;
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
        Vector3 pos = transform.position;       // <— we declare pos once here
        Vector2 mouse = Input.mousePosition;

        //this if function is for debugging purposes
        if (mouse.x <= screenEdgeThickness)
        {
            //Debug.Log($"Pan Left: mouse.x={mouse.x}, edge={screenEdgeThickness}");
            pos.x -= panSpeed * Time.deltaTime;
        }

        if (mouse.x <= screenEdgeThickness)
            pos.x -= panSpeed * Time.deltaTime;
        else if (mouse.x >= Screen.width - screenEdgeThickness)
            pos.x += panSpeed * Time.deltaTime;

        if (mouse.y <= screenEdgeThickness)
            pos.y -= panSpeed * Time.deltaTime;
        else if (mouse.y >= Screen.height - screenEdgeThickness)
            pos.y += panSpeed * Time.deltaTime;

        // ─── CLAMP TO WORLD BOUNDS (dynamic by zoom, with fallback) ──────────────
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float minCamX = minX + horzExtent;
        float maxCamX = maxX - horzExtent;
        float minCamY = minY + vertExtent;
        float maxCamY = maxY - vertExtent;

        // X clamp with fallback if viewport is wider than world
        if (minCamX <= maxCamX)
            pos.x = Mathf.Clamp(pos.x, minCamX, maxCamX);
        else
            pos.x = (minX + maxX) * 0.5f;

        // Y clamp with fallback if viewport is taller than world
        if (minCamY <= maxCamY)
            pos.y = Mathf.Clamp(pos.y, minCamY, maxCamY);
        else
            pos.y = (minY + maxY) * 0.5f;

        // apply the new (clamped) position
        transform.position = pos;
    }


}
