using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SentenceItem
{
    public string type;
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

        foreach (SentenceItem item in data.creativeShortSentences)
        {
            Debug.Log($"[{item.type}] {item.text}");
        }
    }
    public List<SentenceItem> 서바이벌_단문_불러오기()
    {
        return data.survivalShortSentences;
    }

    public List<SentenceItem> 서바이벌_단어_불러오기()
    {
        return data.words;
    }

    public List<string> 크리에이티브_단문_불러오기()
    {
        List<string> result = new();

        foreach (SentenceItem item in data.creativeShortSentences)
        {
            result.Add(item.text);
        }

        return result;
    }

    public List<string> 크리에이티브_장문_불러오기(int num = 1)
    {
        return data.programs[num - 1].code;
    }

    
}