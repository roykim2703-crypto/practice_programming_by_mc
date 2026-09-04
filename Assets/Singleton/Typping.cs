/*
 * 플레이어가 입력한 문자열을 저장하고 다른 스크립트에서 확인할 수 있게 하는 싱글톤입니다.
 *
 * 사용법
 * 1. 빈 GameObject에 이 Typping 스크립트를 추가합니다.
 * 2. TMP_InputField의 On Value Changed (String) 이벤트에
 *    해당 GameObject를 넣고 Typping.SetPlayerText를 선택합니다.
 * 3. 다른 스크립트에서 아래와 같이 입력값을 확인합니다.
 *
 *    string playerText = Typping.Instance.PlayerText;
 *
 * 4. 저장된 입력값을 비울 때는 아래와 같이 사용합니다.
 *
 *    Typping.Instance.ClearPlayerText();
 */
using UnityEngine;

public class Typping : MonoBehaviour
{
    public static Typping Instance { get; private set; }

    public string PlayerText { get; private set; } = string.Empty;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// InputField 또는 TMP_InputField의 On Value Changed 이벤트에 연결합니다.
    /// </summary>
    public void SetPlayerText(string text)
    {
        PlayerText = text ?? string.Empty;
    }

    public void ClearPlayerText()
    {
        PlayerText = string.Empty;
    }
}
