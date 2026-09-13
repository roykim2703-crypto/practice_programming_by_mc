using UnityEngine;
using TMPro;

public class 쓰고있는글 : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    [SerializeField] 써야하는글의코드 textToWrite;

    void Awake()
    {
        inputField.onValueChanged.AddListener(입력바뀜);
    }

    void 입력바뀜(string 현재글)
    {
        Debug.Log("글자 바뀜: " + 현재글);
        textToWrite.Refresh();
    }
}
