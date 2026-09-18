using UnityEngine;

public class BreathingMotion : MonoBehaviour
{
    [SerializeField] public float amplitude = 0.15f;  // 위아래 이동 거리
    [SerializeField] public float speed = 1.5f;       // 속도 (작을수록 느림)
    [SerializeField] private bool useLocalPosition = true;

    private Vector3 startPos;
    private float offset;

    private void Start()
    {
        startPos = useLocalPosition ? transform.localPosition : transform.position;
        offset = Random.Range(0f, Mathf.PI * 2f); // 여러 오브젝트가 동시에 안 움직이도록
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time * speed + offset) * amplitude;
        Vector3 pos = startPos + new Vector3(0f, y, 0f);

        if (useLocalPosition) transform.localPosition = pos;
        else transform.position = pos;
    }
}