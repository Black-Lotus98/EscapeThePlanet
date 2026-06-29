using UnityEngine;
using UnityEngine.UI;

// Self-contained minimap.
//
// An orthographic camera renders the level top-down into a RenderTexture created at
// runtime, which is shown in a corner UI panel. The level is auto-framed from the
// scene's geometry bounds, so the same prefab works in any level with no per-scene
// tuning. A marker tracks the player's position on the map.
//
// Drop the MiniMap prefab into a scene and it just works.
public class MiniMapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera miniMapCamera;
    [SerializeField] private RawImage miniMapImage;      // displays the render texture
    [SerializeField] private RectTransform playerMarker; // "you are here"

    [Header("Settings")]
    [SerializeField] private int textureSize = 512;
    [SerializeField] private float padding = 6f;         // extra world units around the level
    [SerializeField] private float cameraDistance = 200f; // how far back (-Z) the camera sits
    [SerializeField] private string playerTag = "Player";

    private RenderTexture renderTexture;
    private Transform player;
    private Vector2 frameCenter;  // world-space XY centre of the framed area
    private Vector2 frameHalf;    // world-space half extents (x,y) of the framed area
    private bool framed;

    private void Start()
    {
        if (miniMapCamera == null)
        {
            Debug.LogError("MiniMapController: miniMapCamera not assigned.", this);
            enabled = false;
            return;
        }

        renderTexture = new RenderTexture(textureSize, textureSize, 16) { name = "MiniMap_RT" };
        miniMapCamera.targetTexture = renderTexture;
        if (miniMapImage != null) miniMapImage.texture = renderTexture;

        var playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null) player = playerObject.transform;

        // Give the player marker a directional arrow sprite.
        if (playerMarker != null)
        {
            var markerImage = playerMarker.GetComponent<Image>();
            if (markerImage != null) markerImage.sprite = CreateArrowSprite();
        }

        FrameLevel();
    }

    // A simple upward-pointing triangle/arrow, white so it can be tinted via the Image colour.
    private Sprite CreateArrowSprite()
    {
        const int s = 64;
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear };
        var clear = new Color(1f, 1f, 1f, 0f);
        for (int y = 0; y < s; y++)
        {
            float fy = y / (float)(s - 1);          // 0 bottom .. 1 top
            float halfWidth = (1f - fy) * 0.5f;     // apex at top, wide base
            for (int x = 0; x < s; x++)
            {
                float fx = x / (float)(s - 1);
                bool inside = Mathf.Abs(fx - 0.5f) <= halfWidth && fy >= 0.18f; // chop the very bottom for a chevron-ish look
                tex.SetPixel(x, y, inside ? Color.white : clear);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f));
    }

    // Position and size the orthographic camera so the whole level fits.
    private void FrameLevel()
    {
        Bounds bounds = ComputeLevelBounds();
        frameCenter = new Vector2(bounds.center.x, bounds.center.y);

        float halfHeight = bounds.extents.y + padding;
        float halfWidth = bounds.extents.x + padding;
        float aspect = Mathf.Max(miniMapCamera.aspect, 0.0001f);
        float orthoSize = Mathf.Max(halfHeight, halfWidth / aspect);

        frameHalf = new Vector2(orthoSize * aspect, orthoSize);

        miniMapCamera.orthographic = true;
        miniMapCamera.orthographicSize = orthoSize;
        miniMapCamera.transform.position = new Vector3(frameCenter.x, frameCenter.y, bounds.center.z - cameraDistance);
        miniMapCamera.transform.rotation = Quaternion.identity; // looks +Z toward the play plane
        miniMapCamera.nearClipPlane = 0.1f;
        miniMapCamera.farClipPlane = cameraDistance * 2f + bounds.size.z + 10f;

        framed = true;
    }

    // Encapsulate all level geometry (ignores UI, particles, and the minimap itself).
    private Bounds ComputeLevelBounds()
    {
        bool has = false;
        Bounds bounds = new Bounds();

        foreach (var renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (renderer is ParticleSystemRenderer) continue;
            if (!renderer.enabled || !renderer.gameObject.activeInHierarchy) continue;
            if (renderer.GetComponentInParent<Canvas>() != null) continue;          // UI
            if (renderer.GetComponentInParent<MiniMapController>() != null) continue; // self

            if (!has) { bounds = renderer.bounds; has = true; }
            else bounds.Encapsulate(renderer.bounds);
        }

        if (!has) bounds = new Bounds(Vector3.zero, new Vector3(50f, 50f, 1f));
        return bounds;
    }

    private void LateUpdate()
    {
        if (!framed || player == null || playerMarker == null || miniMapImage == null) return;

        Vector2 size = miniMapImage.rectTransform.rect.size;
        float nx = Mathf.Clamp01((player.position.x - (frameCenter.x - frameHalf.x)) / (2f * frameHalf.x));
        float ny = Mathf.Clamp01((player.position.y - (frameCenter.y - frameHalf.y)) / (2f * frameHalf.y));
        playerMarker.anchoredPosition = new Vector2((nx - 0.5f) * size.x, (ny - 0.5f) * size.y);

        // Point the arrow in the ship's heading (its nose = transform.up, in the XY plane).
        Vector3 up = player.up;
        playerMarker.localRotation = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, new Vector2(up.x, up.y)));
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            if (miniMapCamera != null) miniMapCamera.targetTexture = null;
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}
