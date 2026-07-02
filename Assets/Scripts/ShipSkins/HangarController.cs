using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Main-menu hangar: browse ship skins with a live rotating 3D preview and
// save the selection. The preview rig (camera + light + turntable) is built
// at runtime far below the menu scene and rendered to a RawImage, so the
// scene needs no extra permanent objects.
public class HangarController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] RawImage previewImage;
    [SerializeField] TMP_Text shipNameText;
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button selectButton;
    [SerializeField] TMP_Text selectButtonText;
    [Tooltip("Row the colour swatch buttons are built into (hidden when a ship has no variants).")]
    [SerializeField] Transform colorRow;
    [Tooltip("Round sprite used for colour swatches (UI 'Knob').")]
    [SerializeField] Sprite swatchSprite;

    [Header("Preview")]
    [SerializeField] float turnSpeed = 40f;          // deg/sec turntable
    [SerializeField] float fitHeight = 5.5f;         // world units the ship is scaled to fill
    [SerializeField] Vector3 rigPosition = new Vector3(0f, -500f, 0f); // parked far from the menu

    ShipCatalog catalog;
    int index;

    // runtime preview rig
    Camera previewCamera;
    Transform turntable;
    GameObject rigRoot;
    RenderTexture previewTexture;

    void Awake()
    {
        catalog = ShipCatalog.Load();
        prevButton.onClick.AddListener(ShowPrevious);
        nextButton.onClick.AddListener(ShowNext);
        selectButton.onClick.AddListener(SelectCurrent);
    }

    void OnEnable()
    {
        if (catalog == null || catalog.skins.Count == 0)
        {
            Debug.LogWarning("HangarController: no ShipCatalog/skins found.", this);
            return;
        }
        BuildRig();

        ShipSkin current = ShipSelection.Resolve(catalog);
        index = Mathf.Max(0, catalog.IndexOf(current));
        UpdateView();
    }

    void OnDisable()
    {
        TearDownRig();
    }

    void Update()
    {
        if (turntable != null)
            turntable.Rotate(0f, turnSpeed * Time.deltaTime, 0f, Space.World);
    }

    // ------------------------------------------------------------- browsing

    void ShowPrevious()
    {
        index = (index - 1 + catalog.skins.Count) % catalog.skins.Count;
        UpdateView();
    }

    void ShowNext()
    {
        index = (index + 1) % catalog.skins.Count;
        UpdateView();
    }

    void SelectCurrent()
    {
        ShipSkin skin = catalog.skins[index];
        if (!skin.IsUnlocked()) return;
        ShipSelection.SelectedId = skin.id;
        UpdateView();
    }

    void UpdateView()
    {
        ShipSkin skin = catalog.skins[index];

        // rebuild the preview model with the ship's remembered colour
        // (deactivate before the deferred Destroy so two previews never overlap)
        for (int i = turntable.childCount - 1; i >= 0; i--)
        {
            var old = turntable.GetChild(i).gameObject;
            old.SetActive(false);
            Destroy(old);
        }
        GameObject preview = skin.CreatePreview(turntable);
        if (preview != null)
        {
            FitToTurntable(preview);
            ShipColorScheme scheme = skin.GetScheme(ShipSelection.GetColorIndex(skin));
            if (scheme != null) scheme.ApplyTo(preview);
        }

        shipNameText.text = skin.IsUnlocked() ? skin.displayName : skin.displayName + "  🔒";

        bool selected = ShipSelection.Resolve(catalog) == skin;
        selectButton.interactable = !selected && skin.IsUnlocked();
        selectButtonText.text = !skin.IsUnlocked() ? "LOCKED" : (selected ? "SELECTED" : "SELECT");

        RebuildColorRow(skin);
    }

    // One round swatch per colour option; tapping repaints the preview and
    // remembers the choice for this ship.
    void RebuildColorRow(ShipSkin skin)
    {
        if (colorRow == null) return;

        for (int i = colorRow.childCount - 1; i >= 0; i--)
        {
            var old = colorRow.GetChild(i).gameObject;
            old.SetActive(false);
            Destroy(old);
        }

        bool hasVariants = skin.ColorCount > 1 && skin.IsUnlocked();
        colorRow.gameObject.SetActive(hasVariants);
        if (!hasVariants) return;

        int current = ShipSelection.GetColorIndex(skin);
        for (int i = 0; i < skin.ColorCount; i++)
        {
            var go = new GameObject("Swatch_" + i, typeof(RectTransform));
            go.layer = colorRow.gameObject.layer;
            go.transform.SetParent(colorRow, false);

            var img = go.AddComponent<Image>();
            img.sprite = swatchSprite;
            ShipColorScheme scheme = skin.GetScheme(i);
            img.color = scheme != null ? scheme.swatchColor : new Color(0.85f, 0.87f, 0.90f); // default = hull white

            // the layout group sizes children by preferred size; without this the
            // swatches collapse to the tiny Knob sprite's native 32px
            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = 96f;
            le.preferredHeight = 96f;

            bool isCurrent = i == current;
            go.transform.localScale = Vector3.one * (isCurrent ? 1.25f : 0.85f);

            int captured = i;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() =>
            {
                ShipSelection.SetColorIndex(skin, captured);
                UpdateView();
            });
        }
    }

    // Scale/centre the preview so any skin fills the same visual height.
    void FitToTurntable(GameObject preview)
    {
        var renderers = preview.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds b = renderers[0].bounds;
        foreach (var r in renderers) b.Encapsulate(r.bounds);

        float height = Mathf.Max(b.size.y, 0.01f);
        float scale = fitHeight / height;
        preview.transform.localScale *= scale;
        // recentre so the bounds' middle sits on the turntable pivot
        Vector3 offset = (b.center - preview.transform.position) * scale;
        preview.transform.localPosition = -turntable.InverseTransformVector(offset);
    }

    // ---------------------------------------------------------- preview rig

    void BuildRig()
    {
        rigRoot = new GameObject("HangarPreviewRig");
        rigRoot.transform.position = rigPosition;

        var mountGO = new GameObject("Turntable");
        mountGO.transform.SetParent(rigRoot.transform, false);
        turntable = mountGO.transform;

        var camGO = new GameObject("PreviewCamera");
        camGO.transform.SetParent(rigRoot.transform, false);
        camGO.transform.localPosition = new Vector3(0f, 2.5f, -9.5f);
        camGO.transform.LookAt(rigRoot.transform.position + Vector3.up * 0.2f);
        previewCamera = camGO.AddComponent<Camera>();
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = new Color(0.05f, 0.07f, 0.10f, 1f);
        previewCamera.fieldOfView = 38f;
        previewCamera.nearClipPlane = 0.1f;
        previewCamera.farClipPlane = 50f;

        // local light so the preview doesn't depend on the menu scene's lighting
        var lightGO = new GameObject("PreviewLight");
        lightGO.transform.SetParent(rigRoot.transform, false);
        lightGO.transform.localPosition = new Vector3(3f, 5f, -6f);
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 25f;
        light.intensity = 1.6f;

        previewTexture = new RenderTexture(512, 640, 16);
        previewCamera.targetTexture = previewTexture;
        previewImage.texture = previewTexture;
    }

    void TearDownRig()
    {
        if (previewCamera != null) previewCamera.targetTexture = null;
        if (previewTexture != null)
        {
            previewTexture.Release();
            Destroy(previewTexture);
            previewTexture = null;
        }
        if (rigRoot != null) Destroy(rigRoot);
        turntable = null;
        previewCamera = null;
    }
}
