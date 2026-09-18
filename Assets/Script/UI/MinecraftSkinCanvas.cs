using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BreathData
{
    public int speed;
    public float amplitude;
}

public sealed class MinecraftSkinCanvas : MonoBehaviour
{
    private const int PixelScale = 6;
    private const float HeadSideScale = 1f;
    private const float LimbSideScale = 0.95f;
    private const float BodySideScale = 1.05f;

    [SerializeField] private TMP_FontAsset uiFont;
    [SerializeField] private Texture2D defaultSkin;
    [SerializeField] private Vector2 initialPosition = new Vector2(-200f, 0f);
    [SerializeField, Range(0.25f, 3f)] private float avatarScale = 1f;
    [SerializeField] private bool showControls;
    [SerializeField, Min(0)] private float turnSpeedDegrees = 240f;
    [SerializeField] private float initialYaw = -45f;

    [SerializeField] private bool[] breathUpdown = new bool[12];


    [SerializeField] private BreathData[] breathOffset = new BreathData[12];

    private Texture2D skin;
    private bool ownsSkin;
    private MinecraftSkinPartGraphic[] pieces;
    private RectTransform preview;
    private RectTransform controlsPanel;
    private Vector2 targetPosition;
    private float targetYaw;
    private float currentYaw;
    private TextMeshProUGUI status;
    private TextMeshProUGUI armMode;
    private Button armButton;
    private bool slimArms;

    private void Awake()
    {
        BuildUi();
        targetPosition = preview.anchoredPosition;
        currentYaw = targetYaw = NormalizeYaw(initialYaw);
        ApplyYaw();
        if (defaultSkin != null && defaultSkin.width == 64 &&
            (defaultSkin.height == 64 || defaultSkin.height == 32))
        {
            skin = defaultSkin;
            ShowSkin();
            status.text = "기본 스킨: 스티브";
        }
        else
        {
            Debug.LogWarning("MinecraftSkinCanvas needs a 64x64 or 64x32 default skin texture.", this);
        }
    }

    private void Update()
    {
        if (controlsPanel != null && controlsPanel.gameObject.activeSelf != showControls)
            controlsPanel.gameObject.SetActive(showControls);
        if (preview == null)
            return;
        if (!Mathf.Approximately(preview.localScale.x, avatarScale))
            ApplyAvatarScale();
        targetPosition = preview.anchoredPosition;
        float nextYaw = NormalizeYaw(Mathf.MoveTowardsAngle(currentYaw, targetYaw,
            turnSpeedDegrees * Time.deltaTime));
        if (!Mathf.Approximately(nextYaw, currentYaw))
        {
            currentYaw = nextYaw;
            ApplyYaw();
        }
    }

    // Coordinates are relative to the center of the Canvas, in Canvas units.
    public void MoveTo(Vector2 canvasPosition)
    {
        targetPosition = canvasPosition;
        if (preview != null)
            preview.anchoredPosition = canvasPosition;
        else
            initialPosition = canvasPosition;
    }

    public void MoveBy(Vector2 offset) => MoveTo(CurrentPosition + offset);

    public void SetPositionImmediate(Vector2 canvasPosition) => MoveTo(canvasPosition);

    public void SetControlsVisible(bool visible)
    {
        showControls = visible;
        if (controlsPanel != null)
            controlsPanel.gameObject.SetActive(visible);
    }

    public bool ControlsVisible => showControls;
    public Vector2 CurrentPosition => preview != null ? preview.anchoredPosition : initialPosition;

    public float AvatarScale => avatarScale;

    public void SetAvatarScale(float scale)
    {
        avatarScale = Mathf.Clamp(scale, 0.25f, 3f);
        ApplyAvatarScale();
    }

    private void ApplyAvatarScale()
    {
        avatarScale = Mathf.Clamp(avatarScale, 0.25f, 3f);
        if (preview != null)
            preview.localScale = Vector3.one * avatarScale;
    }

    // 0 faces forward, 180 shows the back, and the angle wraps around 360 degrees.
    public void FaceYaw(float degrees) => targetYaw = NormalizeYaw(degrees);

    public void SetYawImmediate(float degrees)
    {
        currentYaw = targetYaw = NormalizeYaw(degrees);
        ApplyYaw();
    }

    private static float NormalizeYaw(float degrees)
    {
        return Mathf.Repeat(degrees + 180f, 360f) - 180f;
    }

    public Vector2 TargetPosition => targetPosition;
    public float TargetYaw => targetYaw;
    public float CurrentYaw => currentYaw;
    public float TurnSpeedDegrees
    {
        get => turnSpeedDegrees;
        set => turnSpeedDegrees = Mathf.Max(0f, value);
    }

    private void ApplyYaw()
    {
        if (pieces == null)
            return;
        float horizontalScale = Mathf.Cos(currentYaw * Mathf.Deg2Rad);
        // A small offset toward the visible side keeps the thicker head from
        // protruding too far ahead of the torso while turning.
        SetPieceX(0, 6, 0.15f * PixelScale * Mathf.Sin(currentYaw * Mathf.Deg2Rad));
        float armCenter = 4f + (skin != null && skin.height == 64 && slimArms ? 1.5f : 2f);
        SetPieceX(2, 8, -armCenter * PixelScale * horizontalScale);
        SetPieceX(3, 9, armCenter * PixelScale * horizontalScale);
        SetPieceX(4, 10, -2f * PixelScale * horizontalScale);
        SetPieceX(5, 11, 2f * PixelScale * horizontalScale);
        int[] drawOrder = currentYaw >= 0f
            ? new[] { 4, 10, 2, 8, 5, 11, 1, 7, 3, 9, 0, 6 }
            : new[] { 5, 11, 3, 9, 4, 10, 1, 7, 2, 8, 0, 6 };
        foreach (int index in drawOrder)
            pieces[index].transform.SetAsLastSibling();
        foreach (MinecraftSkinPartGraphic piece in pieces)
            piece.SetYaw(currentYaw);
    }

    private void SetPieceX(int baseIndex, int overlayIndex, float x)
    {
        SetPieceX(baseIndex, x);
        SetPieceX(overlayIndex, x);
    }

    private void SetPieceX(int index, float x)
    {
        Vector2 position = pieces[index].rectTransform.anchoredPosition;
        position.x = x;
        pieces[index].rectTransform.anchoredPosition = position;
    }

    private void OnDestroy()
    {
        if (ownsSkin && skin != null)
            Destroy(skin);
    }

    private void BuildUi()
    {
        controlsPanel = CreateRect("Skin Controls", transform, new Vector2(180, 102),
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(-12, -12));
        var background = controlsPanel.gameObject.AddComponent<Image>();
        background.color = new Color(0.09f, 0.12f, 0.15f, 0.93f);
        background.raycastTarget = false;

        TMP_FontAsset font = uiFont != null ? uiFont : GetComponentInChildren<TextMeshProUGUI>()?.font;
        CreateLabel("마크 스킨", controlsPanel, font, 17, new Vector2(110, 28), new Vector2(-22, 28));
        var armRect = CreateRect("Arm Width", controlsPanel, new Vector2(55, 28),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(61, 28));
        var armImage = armRect.gameObject.AddComponent<Image>();
        armImage.color = new Color(0.22f, 0.29f, 0.35f);
        armButton = armRect.gameObject.AddComponent<Button>();
        armButton.targetGraphic = armImage;
        armButton.onClick.AddListener(ToggleArmWidth);
        armMode = CreateLabel("팔 4px", armRect, font, 10, new Vector2(55, 28), Vector2.zero);

        preview = CreateRect("Avatar Skin", transform, new Vector2(120, 200),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), initialPosition);
        ApplyAvatarScale();
        pieces = new MinecraftSkinPartGraphic[12];
        pieces[0] = CreatePiece("Head", preview, 8, 8, 0, 12);
        pieces[1] = CreatePiece("Body", preview, 8, 12, 0, 2);
        pieces[2] = CreatePiece("Right Arm", preview, 4, 12, -6, 2);
        pieces[3] = CreatePiece("Left Arm", preview, 4, 12, 6, 2);
        pieces[4] = CreatePiece("Right Leg", preview, 4, 12, -2, -10);
        pieces[5] = CreatePiece("Left Leg", preview, 4, 12, 2, -10);
        pieces[6] = CreatePiece("Hat", preview, 8, 8, 0, 12);
        pieces[7] = CreatePiece("Jacket", preview, 8, 12, 0, 2);
        pieces[8] = CreatePiece("Right Sleeve", preview, 4, 12, -6, 2);
        pieces[9] = CreatePiece("Left Sleeve", preview, 4, 12, 6, 2);
        pieces[10] = CreatePiece("Right Pants", preview, 4, 12, -2, -10);
        pieces[11] = CreatePiece("Left Pants", preview, 4, 12, 2, -10);
        // Far limbs sit behind the body; each outer layer stays over its own base.
        foreach (int index in new[] { 4, 10, 2, 8, 5, 11, 1, 7, 3, 9, 0, 6 })
            pieces[index].transform.SetAsLastSibling();
        for (int i = 0; i < pieces.Length; i++)
            if (breathUpdown[i])
            {
                pieces[i].gameObject.AddComponent<BreathingMotion>();
                var breath = pieces[i].gameObject.GetComponent<BreathingMotion>();
                breath.speed = breathOffset[i].speed;
                breath.amplitude = breathOffset[i].amplitude;
            } 
            
        
            
        
        

        var buttonRect = CreateRect("Upload Skin", controlsPanel, new Vector2(150, 34),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -5));
        var buttonImage = buttonRect.gameObject.AddComponent<Image>();
        buttonImage.color = new Color(0.25f, 0.54f, 0.31f);
        var button = buttonRect.gameObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(ChooseSkin);
        CreateLabel("스킨 PNG 선택", buttonRect, font, 14, new Vector2(150, 34), Vector2.zero);

        status = CreateLabel("64×64 또는 64×32 PNG", controlsPanel, font, 10,
            new Vector2(164, 15), new Vector2(0, -40));
        status.overflowMode = TextOverflowModes.Ellipsis;
        controlsPanel.gameObject.SetActive(showControls);
    }

    public void ChooseSkin()
    {
        status.text = "PNG 선택 중";
        string path = OpenSkinFile();
        if (string.IsNullOrEmpty(path))
        {
            status.text = "PNG 선택이 취소됨";
            return;
        }

        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            var loaded = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!loaded.LoadImage(bytes) || loaded.width != 64 || (loaded.height != 64 && loaded.height != 32))
            {
                Destroy(loaded);
                status.text = "64×64 / 64×32 PNG만 가능";
                return;
            }

            loaded.filterMode = FilterMode.Point;
            loaded.wrapMode = TextureWrapMode.Clamp;
            if (ownsSkin && skin != null)
                Destroy(skin);
            skin = loaded;
            ownsSkin = true;
            if (skin.height == 32)
            {
                slimArms = false;
                armMode.text = "팔 4px";
            }
            armButton.interactable = skin.height == 64;
            ShowSkin();
            status.text = Path.GetFileName(path);
        }
        catch (Exception exception)
        {
            status.text = "파일을 열 수 없습니다";
            Debug.LogWarning($"Minecraft skin could not be loaded: {exception.Message}");
        }
    }

    private void ShowSkin()
    {
        bool modern = skin.height == 64;
        int armWidth = modern && slimArms ? 3 : 4;
        SetPiece(0, Face(8, 8, 8, 8), Face(16, 8, 8, 8));
        SetPiece(1, Face(20, 20, 8, 12), Face(28, 20, 4, 12));
        SetPiece(2, Face(44, 20, armWidth, 12), Face(44 + armWidth, 20, 4, 12));
        SetPiece(3, Face(modern ? 36 : 44, modern ? 52 : 20, armWidth, 12),
            Face(modern ? 36 + armWidth : 40, modern ? 52 : 20, 4, 12), !modern);
        SetPiece(4, Face(4, 20, 4, 12), Face(8, 20, 4, 12));
        SetPiece(5, Face(modern ? 20 : 4, modern ? 52 : 20, 4, 12),
            Face(modern ? 24 : 0, modern ? 52 : 20, 4, 12), !modern);
        SetPiece(6, Face(40, 8, 8, 8), Face(48, 8, 8, 8));
        SetPiece(7, Face(20, 36, 8, 12), Face(28, 36, 4, 12),
            false, modern);
        SetPiece(8, Face(44, 36, armWidth, 12), Face(44 + armWidth, 36, 4, 12),
            false, modern);
        SetPiece(9, Face(52, 52, armWidth, 12), Face(52 + armWidth, 52, 4, 12),
            false, modern);
        SetPiece(10, Face(4, 36, 4, 12), Face(8, 36, 4, 12),
            false, modern);
        SetPiece(11, Face(4, 52, 4, 12), Face(8, 52, 4, 12),
            false, modern);
        ApplyYaw();
    }

    private static RectInt Face(int x, int y, int width, int height)
    {
        return new RectInt(x, y, width, height);
    }

    private void SetPiece(int index, RectInt front, RectInt side,
        bool mirror = false, bool visible = true)
    {
        MinecraftSkinPartGraphic graphic = pieces[index];
        graphic.gameObject.SetActive(visible);
        if (!visible)
            return;
        float sideScale = index == 0 || index == 6 ? HeadSideScale :
            index == 1 || index == 7 ? BodySideScale : LimbSideScale;
        graphic.Configure(skin, front, side, PixelScale, mirror, sideScale);
    }

    private void ToggleArmWidth()
    {
        slimArms = !slimArms;
        armMode.text = slimArms ? "팔 3px" : "팔 4px";
        if (skin != null)
            ShowSkin();
    }

    private static RectTransform CreateRect(string name, Transform parent, Vector2 size,
        Vector2 anchor, Vector2 pivot, Vector2 position)
    {
        var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.gameObject.layer = parent.gameObject.layer;
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return rect;
    }

    private static MinecraftSkinPartGraphic CreatePiece(string name, Transform parent,
        int width, int height,
        int x, int y)
    {
        var rect = CreateRect(name, parent, new Vector2(width, height) * PixelScale,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(x, y) * PixelScale);
        var graphic = rect.gameObject.AddComponent<MinecraftSkinPartGraphic>();
        graphic.raycastTarget = false;
        graphic.gameObject.SetActive(false);
        return graphic;
    }

    private static TextMeshProUGUI CreateLabel(string value, Transform parent, TMP_FontAsset font,
        float size, Vector2 dimensions, Vector2 position)
    {
        var rect = CreateRect(value, parent, dimensions, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), position);
        var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.text = value;
        label.font = font;
        label.fontSize = size;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        label.raycastTarget = false;
        return label;
    }

    private static string OpenSkinFile()
    {
#if UNITY_EDITOR
        return UnityEditor.EditorUtility.OpenFilePanel("Minecraft skin PNG", "", "png");
#elif UNITY_STANDALONE_WIN
        var file = new StringBuilder(1024);
        var dialog = new OpenFileName
        {
            size = Marshal.SizeOf<OpenFileName>(),
            filter = "PNG files (*.png)\0*.png\0\0",
            file = file,
            maxFile = file.Capacity,
            title = "Minecraft skin PNG",
            flags = 0x00001000 | 0x00000008 // File must exist; path must exist.
        };
        return GetOpenFileName(ref dialog) ? file.ToString() : null;
#else
        Debug.LogWarning("Skin file selection is supported in the Editor and Windows builds.");
        return null;
#endif
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct OpenFileName
    {
        public int size;
        public IntPtr owner;
        public IntPtr instance;
        [MarshalAs(UnmanagedType.LPWStr)] public string filter;
        public IntPtr customFilter;
        public int maxCustomFilter;
        public int filterIndex;
        [MarshalAs(UnmanagedType.LPWStr)] public StringBuilder file;
        public int maxFile;
        public IntPtr fileTitle;
        public int maxFileTitle;
        public IntPtr initialDirectory;
        [MarshalAs(UnmanagedType.LPWStr)] public string title;
        public int flags;
        public short fileOffset;
        public short fileExtension;
        public IntPtr defaultExtension;
        public IntPtr customData;
        public IntPtr hook;
        public IntPtr templateName;
        public IntPtr reserved;
        public int reservedSize;
        public int flagsEx;
    }

    [DllImport("comdlg32.dll", EntryPoint = "GetOpenFileNameW", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetOpenFileName(ref OpenFileName dialog);
#endif
}
