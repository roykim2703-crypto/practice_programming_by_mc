/*
 * Suvive, HardCore, Develop 모드의 점수를 각각 float 형식으로 저장하는 싱글톤입니다.
 *
 * 사용법
 *
 * 1. 점수 설정
 * SavePoint.Instance.SetSuviveScore(100.5f);
 * SavePoint.Instance.SetHardCoreScore(200f);
 * SavePoint.Instance.SetDevelopScore(300f);
 *
 * 2. 현재 점수에 더하기
 * SavePoint.Instance.AddSuviveScore(10f);
 * SavePoint.Instance.AddHardCoreScore(10f);
 * SavePoint.Instance.AddDevelopScore(10f);
 *
 * 3. 점수 확인
 * float suviveScore = SavePoint.Instance.SuviveScore;
 * float hardCoreScore = SavePoint.Instance.HardCoreScore;
 * float developScore = SavePoint.Instance.DevelopScore;
 *
 * 4. 점수 초기화
 * SavePoint.Instance.ClearSuviveScore();
 * SavePoint.Instance.ClearHardCoreScore();
 * SavePoint.Instance.ClearDevelopScore();
 * SavePoint.Instance.ClearAllScores(); // 세 모드 점수를 모두 초기화
 */
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public static SavePoint Instance { get; private set; }

    public float SuviveScore { get; private set; }
    public float HardCoreScore { get; private set; }
    public float DevelopScore { get; private set; }

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

    public void SetSuviveScore(float score)
    {
        SuviveScore = score;
    }

    public void SetHardCoreScore(float score)
    {
        HardCoreScore = score;
    }

    public void SetDevelopScore(float score)
    {
        DevelopScore = score;
    }

    public void AddSuviveScore(float amount)
    {
        SuviveScore += amount;
    }

    public void AddHardCoreScore(float amount)
    {
        HardCoreScore += amount;
    }

    public void AddDevelopScore(float amount)
    {
        DevelopScore += amount;
    }

    public void ClearSuviveScore()
    {
        SuviveScore = 0f;
    }

    public void ClearHardCoreScore()
    {
        HardCoreScore = 0f;
    }

    public void ClearDevelopScore()
    {
        DevelopScore = 0f;
    }

    public void ClearAllScores()
    {
        SuviveScore = 0f;
        HardCoreScore = 0f;
        DevelopScore = 0f;
    }
}
