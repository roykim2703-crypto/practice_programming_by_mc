using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class 쓰고있는글 : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] 써야하는글의코드 textToWrite;
    [SerializeField] 스탯기록용 stat;

    void Awake()
    {
        inputField.onValueChanged.AddListener(입력바뀜);

        if (!stat)
            stat = FindAnyObjectByType<스탯기록용>();
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            엔터누름();
        }
    }

    void 입력바뀜(string 현재글)
    {
        textToWrite.Refresh();
        stat.입력시작();
    }

    void 엔터누름()
    {
        //Debug.Log("엔터!");
        
        inputField.text = inputField.text + " ";

        
    }
}