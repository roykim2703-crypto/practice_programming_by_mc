using System.Collections;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class 시작카운트다운 : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI count;
    [SerializeField] GameObject 타자입력하는곳;

    [SerializeField] float sec = 0.7f;

    Image image;
    void Awake()
    {
        image = GetComponent<Image>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(startCountdown());
    }

    IEnumerator startCountdown()
    {
        count.text="3";
        yield return new WaitForSeconds(sec);
        count.text="2";
        yield return new WaitForSeconds(sec);
        count.text="1";
        yield return new WaitForSeconds(sec);
        count.text="start!";
        yield return new WaitForSeconds(sec/2);
        image.enabled = false;
        count.text = "";
        타자입력하는곳.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}