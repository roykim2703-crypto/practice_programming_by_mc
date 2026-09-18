using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SentenceItem
{
    public string[] tags;
    public string text;
}

[Serializable]
public class SentenceData
{
    public List<SentenceItem> words;
    public List<SentenceItem> survivalShortSentences;
    public List<SentenceItem> creativeShortSentences;
    public List<ProgramItem> programs;
}

[Serializable]
public class ProgramItem
{
    public string name;
    public string description;
    public List<string> code;
}

public class 문장불러오기 : MonoBehaviour
{
    [SerializeField] public bool 서바이벌단어사용함 = false;
    [SerializeField] public bool 서바이벌단문사용함 = false;
    [SerializeField] public bool 크리에이티브단문사용함 = false;
    [SerializeField] public bool 크리에이티브장문사용함 = false;
    private SentenceData data;

    void Start()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("tajaDATA");

        if (jsonFile == null)
        {
            Debug.LogError("Resources/tajaDATA.json 을 찾을 수 없음");
            return;
        }

        data = JsonUtility.FromJson<SentenceData>(jsonFile.text);

        // foreach (SentenceItem item in data.creativeShortSentences)
        // {
        //     Debug.Log($"[{item.type}] {item.text}");
        // }
    }
    /// <summary>
    /// 이거 크리에이티브 장문은 안나옴
    /// </summary>
    /// <param name="tags">원하는 태그 있으면 여기에 쓰셈. 그러면 그 태그를 포함하고 있는것 반환함</param>
    /// <returns></returns>
    public SentenceItem 랜덤문장불러오기(params string[] tags)
    {
        SentenceItem result;
        List<SentenceItem> 활성화한전체집합 = new();
        if (서바이벌단어사용함) 활성화한전체집합.AddRange(data.words);
        if (서바이벌단문사용함) 활성화한전체집합.AddRange(data.survivalShortSentences);
        if (크리에이티브단문사용함) 활성화한전체집합.AddRange(data.creativeShortSentences);

        List<SentenceItem> 태그로분류한집합 = new();
        if (tags.Length > 0)
        //만약 파라미터로 태그를 주면
            foreach(var element in 활성화한전체집합)
            {
                bool 태그거기있음 = true;
                foreach(var elementTag in tags)
                {
                    if (!element.tags.Contains(elementTag))
                    {
                        태그거기있음 = false;
                        break;
                    
                    } 
                }
                if (태그거기있음) 태그로분류한집합.Add(element);
            }
        else 태그로분류한집합.AddRange(활성화한전체집합);
        
        System.Random index = new System.Random();
        result = 태그로분류한집합[index.Next(0,태그로분류한집합.Count)];
        return result;
    }
    public List<SentenceItem> 서바이벌_단어_불러오기()
    {
        return data.words;
    }
    public List<SentenceItem> 서바이벌_단문_불러오기()
    {
        return data.survivalShortSentences;
    }

    public List<SentenceItem> 크리에이티브_단문_불러오기()
    {
        // List<string> result = new();

        // foreach (SentenceItem item in data.creativeShortSentences)
        // {
        //     result.Add(item.text);
        // }

        return data.creativeShortSentences;
    }

    // public SentenceItem 크리에이티브_장문_불러오기(string name, int index=1)
    // {
    //     foreach (ProgramItem program in data.programs)
    //     {
    //         if (program.name == name)
    //         {
    //             return program.code[index];
    //         }
    //     }

    //     return new List<string>();
    // }이건 나중에 만든다
    
}