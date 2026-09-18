using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//사용법
//얘는 써야하는 글(canvas 오브젝트)의 글 내용으로 자동으로 판단함
//그래서 쓸 글을 바꿔야 한다면 오브젝트의 글 내용만 바꾸면 됨

public class 써야하는글의코드 : MonoBehaviour
{
    [SerializeField] bool 크리에이티브장문 = false;
    [SerializeField] int 몇개씀 = 0;
    [SerializeField] TMP_InputField 입력글오브젝트;
    [SerializeField] RectTransform 써야할글자Rect;

    [SerializeField] int 이동시작글자 = 10;
    [SerializeField] 스탯기록용 stat;
    [SerializeField] 문장불러오기 문장;
    public int 오타수 = 0;

    Vector2 처음위치;
    
    TextMeshProUGUI 써야할글자;
    void Awake()
    {
        써야할글자 = GetComponent<TextMeshProUGUI>();
        써야할글자Rect = GetComponent<RectTransform>();
        if (!문장) 문장 = FindAnyObjectByType<문장불러오기>();
        처음위치 = 써야할글자Rect.anchoredPosition;
    }

    // 글자를 쓸 때마다 호출
    public void Refresh()
    {
        오타수 = 0;
        써야할글자.ForceMeshUpdate();

        string 입력글 = 입력글오브젝트.text;
        string 정답글 = 써야할글자.text;

        //Debug.Log("입력글오브젝트 : "+입력글.Length);

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
        int 현재글자 = 검사할길이-1;

        if (현재글자 >= 0)
        {
            if (입력글[현재글자] == 정답글[현재글자])
                글자색바꾸기(현재글자+1, Color.yellow);
        }

        써야할글자.UpdateVertexData(
            TMP_VertexDataUpdateFlags.Colors32
        );

        위치갱신();

        int 써야할글자길이 = 써야할글자.text.Length;

        if (입력글.Length > 써야할글자길이 || (입력글.Length >= 써야할글자길이) && Keyboard.current.enterKey.isPressed)
        {
            춘배야다음글받아오거라();    
        }
        //테스트용
        //입력글오브젝트.text="";
    }
    
    private void 글자색바꾸기(int index, Color32 color)
    {
        if (써야할글자.text.Length-1 < index) return;
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
        TMP_TextInfo 글자정보 = 써야할글자.textInfo;
        int 끝글자 = Mathf.Min(입력글오브젝트.text.Length, 글자정보.characterCount);
        int 시작글자 = Mathf.Clamp(이동시작글자, 0, 글자정보.characterCount);

        if (끝글자 <= 시작글자)
        {
            써야할글자Rect.anchoredPosition = 처음위치;
            return;
        }

        float 시작위치 = 시작글자 == 0
            ? 글자정보.characterInfo[0].origin
            : 글자정보.characterInfo[시작글자 - 1].xAdvance;
        float 끝위치 = 글자정보.characterInfo[끝글자 - 1].xAdvance;
        float 이동거리 = Mathf.Max(0f, 끝위치 - 시작위치);

        써야할글자Rect.anchoredPosition =
            처음위치 + Vector2.left * 이동거리;
    }

    public void 춘배야다음글받아오거라()
    {
        춘배야지금쓰는중인거초기화해라();
        춘배야써야하는글보여주는거초기화해라();
    }

    public void 춘배야지금쓰는중인거초기화해라()
    {
        stat.완료한입력글자수 += 써야할글자.text.Length;
        stat.전체오타수 += 오타수;
        오타수 = 0;
        입력글오브젝트.text = "";
        몇개씀++;
        StartCoroutine(다시글쓰기시작());   
    }

    IEnumerator 다시글쓰기시작()
    {
        yield return null;
        입력글오브젝트.ActivateInputField();
    }
    public void 춘배야써야하는글보여주는거초기화해라()
    {
        if (크리에이티브장문)
        {
            
        }
        else
        {
            써야할글자.text=문장.랜덤문장불러오기().text;
        }
        
        
    }
}
