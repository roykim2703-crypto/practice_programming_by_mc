using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CanvasRenderer))]
public sealed class MinecraftSkinPartGraphic : RawImage
{
    private Texture2D skin;
    private RectInt front;
    private RectInt back;
    private RectInt rightSide;
    private RectInt leftSide;
    private bool mirror;
    private int pixelScale;
    private float sideProjectionScale;
    private float yaw;

    public void SetYaw(float degrees)
    {
        if (Mathf.Approximately(yaw, degrees))
            return;
        yaw = degrees;
        SetVerticesDirty();
    }

    public void Configure(Texture2D texture, RectInt frontFace, RectInt sideFace,
        int scale, bool mirrorFront, float sideScale)
    {
        skin = texture;
        this.texture = texture;
        front = frontFace;
        rightSide = sideFace;
        leftSide = new RectInt(mirrorFront ? frontFace.x + frontFace.width :
            frontFace.x - sideFace.width, frontFace.y, sideFace.width, sideFace.height);
        back = new RectInt(mirrorFront ? frontFace.x + frontFace.width + sideFace.width :
            sideFace.x + sideFace.width, frontFace.y, frontFace.width, frontFace.height);
        pixelScale = scale;
        mirror = mirrorFront;
        sideProjectionScale = sideScale;

        rectTransform.sizeDelta = new Vector2((front.width + sideFace.width * sideScale) * scale,
            front.height * scale);
        SetVerticesDirty();
        SetMaterialDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vertices)
    {
        vertices.Clear();
        if (skin == null)
            return;

        float radians = yaw * Mathf.Deg2Rad;
        float cosine = Mathf.Cos(radians);
        float sine = Mathf.Sin(radians);
        float width = front.width * pixelScale;
        float height = front.height * pixelScale;
        float halfSide = rightSide.width * pixelScale * sideProjectionScale * 0.5f;
        float frontLeft = -width * 0.5f * cosine - halfSide * sine;
        float frontRight = width * 0.5f * cosine - halfSide * sine;
        float backLeft = -width * 0.5f * cosine + halfSide * sine;
        float backRight = width * 0.5f * cosine + halfSide * sine;
        float bottom = -height * 0.5f;
        float upper = bottom + height;
        if (Mathf.Abs(sine) > 0.001f)
        {
            if (sine > 0f)
                AddFace(vertices, rightSide, new Color32(175, 175, 175, 255), false,
                    new Vector2(frontRight, bottom), new Vector2(frontRight, upper),
                    new Vector2(backRight, upper), new Vector2(backRight, bottom));
            else
                AddFace(vertices, leftSide, new Color32(175, 175, 175, 255), false,
                    new Vector2(backLeft, bottom), new Vector2(backLeft, upper),
                    new Vector2(frontLeft, upper), new Vector2(frontLeft, bottom));
        }
        if (Mathf.Abs(cosine) > 0.001f)
        {
            if (cosine > 0f)
                AddFace(vertices, front, new Color32(255, 255, 255, 255), mirror,
                    new Vector2(frontLeft, bottom), new Vector2(frontLeft, upper),
                    new Vector2(frontRight, upper), new Vector2(frontRight, bottom));
            else
                AddFace(vertices, back, new Color32(235, 235, 235, 255), mirror,
                    new Vector2(backRight, bottom), new Vector2(backRight, upper),
                    new Vector2(backLeft, upper), new Vector2(backLeft, bottom));
        }
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
