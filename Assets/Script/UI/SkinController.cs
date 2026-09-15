using UnityEngine;

public class SkinController : MonoBehaviour
{
    [SerializeField] private MinecraftSkinCanvas skin;
    public float rotation = 45f;
    void Start()
    {
        skin.FaceYaw(45f);                 // 오른쪽 바라보기
        skin.MoveTo(new Vector2(20, 0));  // 패널 중심에서 오른쪽으로 이동
    }
    void Update()
    {
        skin.FaceYaw(rotation);
    }
}