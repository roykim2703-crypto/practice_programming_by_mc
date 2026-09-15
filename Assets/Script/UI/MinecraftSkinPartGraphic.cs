using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasRenderer))]
public sealed class MinecraftSkinPartGraphic : RawImage
{
    private Texture2D skin;
    private RectInt front;
    private RectInt rightSide;
    private RectInt leftSide;
    private RectInt top;
    private bool mirror;
    private int pixelScale;
    private float yaw;

    public void SetYaw(float degrees)
    {
        if (Mathf.Approximately(yaw, degrees))
            return;
        yaw = degrees;
        SetVerticesDirty();
    }

    public void Configure(Texture2D texture, RectInt frontFace, RectInt sideFace,
        RectInt topFace, int scale, bool mirrorFront)
    {
        skin = texture;
        this.texture = texture;
        front = frontFace;
        rightSide = sideFace;
        leftSide = new RectInt(frontFace.x - sideFace.width, frontFace.y,
            sideFace.width, sideFace.height);
        top = topFace;
        pixelScale = scale;
        mirror = mirrorFront;

        rectTransform.sizeDelta = new Vector2((front.width + sideFace.width) * scale,
            (front.height + topFace.height) * scale);
        SetVerticesDirty();
        SetMaterialDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vertices)
    {
        vertices.Clear();
        if (skin == null)
            return;

        float radians = yaw * Mathf.Deg2Rad;
        float width = front.width * pixelScale * Mathf.Cos(radians);
        float height = front.height * pixelScale;
        float sideWidth = rightSide.width * pixelScale * Mathf.Abs(Mathf.Sin(radians)) * 0.72f;
        float topRise = top.height * pixelScale * Mathf.Abs(Mathf.Sin(radians)) * 0.22f;
        float left = -(width + sideWidth) * 0.5f;
        float right = left + width;
        float bottom = -height * 0.5f;
        float upper = bottom + height;
        bool turnsRight = yaw >= 0f;
        if (sideWidth > 0.001f)
        {
            if (turnsRight)
                AddFace(vertices, rightSide, new Color32(175, 175, 175, 255), false,
                    new Vector2(right, bottom), new Vector2(right, upper),
                    new Vector2(right + sideWidth, upper + topRise),
                    new Vector2(right + sideWidth, bottom + topRise));
            else
                AddFace(vertices, leftSide, new Color32(175, 175, 175, 255), false,
                    new Vector2(left - sideWidth, bottom + topRise),
                    new Vector2(left - sideWidth, upper + topRise),
                    new Vector2(left, upper), new Vector2(left, bottom));
            AddFace(vertices, top, new Color32(225, 225, 225, 255), false,
                turnsRight ? new Vector2(left, upper) : new Vector2(left - sideWidth, upper + topRise),
                turnsRight ? new Vector2(left + sideWidth, upper + topRise) : new Vector2(left, upper),
                turnsRight ? new Vector2(right + sideWidth, upper + topRise) : new Vector2(right, upper),
                turnsRight ? new Vector2(right, upper) : new Vector2(right - sideWidth, upper + topRise));
        }
        AddFace(vertices, front, new Color32(255, 255, 255, 255), mirror,
            new Vector2(left, bottom), new Vector2(left, upper),
            new Vector2(right, upper), new Vector2(right, bottom));
    }

    private void AddFace(VertexHelper vertices, RectInt source, Color32 shade, bool flip,
        Vector2 bottomLeft, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight)
    {
        float u0 = (float)source.x / skin.width;
        float u1 = (float)(source.x + source.width) / skin.width;
        float v0 = (float)(skin.height - source.y - source.height) / skin.height;
        float v1 = (float)(skin.height - source.y) / skin.height;
        if (flip)
        {
            float swap = u0;
            u0 = u1;
            u1 = swap;
        }

        int first = vertices.currentVertCount;
        vertices.AddVert(bottomLeft, shade, new Vector2(u0, v0));
        vertices.AddVert(topLeft, shade, new Vector2(u0, v1));
        vertices.AddVert(topRight, shade, new Vector2(u1, v1));
        vertices.AddVert(bottomRight, shade, new Vector2(u1, v0));
        vertices.AddTriangle(first, first + 1, first + 2);
        vertices.AddTriangle(first, first + 2, first + 3);
    }
}
