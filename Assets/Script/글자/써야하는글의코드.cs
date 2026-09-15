using TMPro;
using UnityEngine;

//사용법
//얘는 써야하는 글(canvas 오브젝트)의 글 내용으로 자동으로 판단함
//그래서 쓸 글을 바꿔야 한다면 오브젝트의 글 내용만 바꾸면 됨

public class 써야하는글의코드 : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI inputText;
    [SerializeField] RectTransform 써야할글자Rect;

    [SerializeField] int 이동시작글자 = 10;
    [SerializeField] float 글자당이동거리 = 18f;

    public int 오타수 = 0;

    Vector2 처음위치;

    TextMeshProUGUI 써야할글자;
    void Awake()
    {
        써야할글자 = GetComponent<TextMeshProUGUI>();
        써야할글자Rect = GetComponent<RectTransform>();

        처음위치 = 써야할글자Rect.anchoredPosition;
    }

    // 글자를 쓸 때마다 호출
    public void Refresh()
    {
        오타수 = -1;
        써야할글자.ForceMeshUpdate();

        string 입력글 = inputText.text;
        string 정답글 = 써야할글자.text;

        // 아무것도 입력 안 했을 때
        if (입력글.Length == 0)
        {
            써야할글자Rect.anchoredPosition = 처음위치;
            return;
        }

        int 검사할길이 = Mathf.Min(
            입력글.Length,
            정답글.Length
        );

        for (int i = 0; i < 검사할길이; i++)
        {
            if (입력글[i] == 정답글[i])
                글자색바꾸기(i, Color.white);
            //오타 났을때
            else
            {
                글자색바꾸기(i, Color.red);
                오타수++;
            }
                
        }

        // 현재 입력한 마지막 글자는 노랑
        int 현재글자 = 검사할길이 - 1;

        if (현재글자 >= 0)
        {
            if (입력글[현재글자] == 정답글[현재글자])
                글자색바꾸기(현재글자, Color.white);
            else
                글자색바꾸기(현재글자, Color.yellow);
        }

        써야할글자.UpdateVertexData(
            TMP_VertexDataUpdateFlags.Colors32
        );

        위치갱신();
    }
    
    private void 글자색바꾸기(int index, Color32 color)
    {
        TMP_CharacterInfo charInfo =
            써야할글자.textInfo.characterInfo[index];

        if (!charInfo.isVisible)
            return;

        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        Color32[] colors =
            써야할글자.textInfo.meshInfo[materialIndex].colors32;

        colors[vertexIndex] = color;
        colors[vertexIndex + 1] = color;
        colors[vertexIndex + 2] = color;
        colors[vertexIndex + 3] = color;
    }

    public void 위치갱신()
    {
        int len = inputText.text.Length;

        int 이동글자수 = Mathf.Max(0, len - 이동시작글자);

        float 이동거리 = 이동글자수 * 글자당이동거리;

        써야할글자Rect.anchoredPosition =
            처음위치 + Vector2.left * 이동거리;
    }
}