using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class 스탯기록용 : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI inputText;

    [SerializeField] TextMeshProUGUI 타자수보여주는거;
    [SerializeField] TextMeshProUGUI 오타율;
    [SerializeField] TextMeshProUGUI 경과시간보여주는텍스트오브젝트;
    [SerializeField] public 써야하는글의코드 오타수코드;

    private bool isStart = false;
    public bool 시작함 => isStart;

    //남은 시간 : 
    private float time = 0;
    public float 경과시간 => time;

    private int sec = 0;
    public int 초 => sec;
    
    private int min = 0;
    public int 분 => min;

    private int taja = 0;
    public int 타자수 => taja;

    public int 전체오타수 = 0;

    private float accuracy = 0;
    public float 정확도 => accuracy;

    public int 완료한입력글자수 = 0;


    int intTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!오타수코드) 오타수코드 = FindAnyObjectByType<써야하는글의코드>();
    }

    public void 입력시작()
    {
        isStart = true;
    }

    // Update is called once per frame
    void Update()
    {   
        if (!isStart) return;
        time += Time.deltaTime;

        intTime = (int) Math.Floor(time);

        sec = intTime % 60;

        min = intTime / 60;

        try
        {
            taja = (int)((완료한입력글자수+inputText.text.Length-1 - (오타수코드.오타수+전체오타수)) / time * 60);
            accuracy = 100 - (float)(오타수코드.오타수+전체오타수) / (완료한입력글자수+inputText.text.Length-1) * 100;
            //Debug.Log("오타수:"+오타수코드.오타수 + 전체오타수);
            //Debug.Log(inputText.text.Length-1);
            if (accuracy < 50) taja = 0;
        }
        catch (NullReferenceException)
        {
            Debug.Log(gameObject.name + "의 \'스탯기록용\' 컴포넌트 중 \'inputText\' 채우세요");
        }


        //try catch는 예외 처리하는 애임. 배워두면 좋음.
        try
        {
            타자수보여주는거.text = ""+taja;
        }
        catch (NullReferenceException)
        {
            Debug.Log(gameObject.name + "의 \'스탯기록용\' 컴포넌트 중 \'타자 수 보여주는거\' 채우세요");
        }


        try
        {
            오타율.text = Mathf.Floor(accuracy*10)/10+"%";
        }
        catch (NullReferenceException)
        {
            Debug.Log(gameObject.name + "의 \'스탯기록용\' 컴포넌트 중 \'타자 수 보여주는거\' 채우세요");
        }


        try
        {
            경과시간보여주는텍스트오브젝트.text = min+":"+sec;
        }
        catch (NullReferenceException)
        {
            Debug.Log(gameObject.name + "의 \'스탯기록용\' 컴포넌트 중 \'타자 수 보여주는거\' 채우세요");
        }
        
    }

}
