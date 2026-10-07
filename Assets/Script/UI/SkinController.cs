using UnityEngine;

public class SkinController : MonoBehaviour
{
    [SerializeField] private MinecraftSkinCanvas skin;
    [Tooltip("Canvas 가운데 기준 위치. X가 작을수록 왼쪽입니다.")]
    public Vector2 position = new Vector2(-200f, 0f);
    [Range(0.25f, 3f)] public float size = 1f;
    public float rotation = -45f;
    private Vector2 lastAppliedPosition;
    private float lastAppliedSize;
    private float lastAppliedRotation;

    void Start()
    {
        lastAppliedPosition = position;
        lastAppliedSize = size;
        lastAppliedRotation = rotation;
        if (skin == null)
            return;
        skin.SetPositionImmediate(position);
        skin.SetAvatarScale(size);
        skin.SetYawImmediate(rotation);
    }

    void Update()
    {
        if (skin == null)
            return;
        if (position != lastAppliedPosition)
        {
            lastAppliedPosition = position;
            skin.MoveTo(position);
        }
        if (!Mathf.Approximately(size, lastAppliedSize))
        {
            lastAppliedSize = size;
            skin.SetAvatarScale(size);
        }
        if (!Mathf.Approximately(rotation, lastAppliedRotation))
        {
            lastAppliedRotation = rotation;
            skin.FaceYaw(rotation);
        }
    }

    public void Attack()
    {
        if (skin != null)
            skin.Attack();
    }

    public void Jump(float n)
    {
        if (skin != null)
            skin.Jump(n);
    }

    public void Jump()
    {
        if (skin != null)
            skin.Jump();
    }

    // Lowercase aliases for scripts that call attack() / jump().
    public void attack() => Attack();
    public void jump(float n) => Jump(n);
    public void jump() => Jump();
}
