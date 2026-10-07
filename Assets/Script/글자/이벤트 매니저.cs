using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SkinController))]
public class 이벤트매니저 : MonoBehaviour
{
    SkinController skinController;

    void Awake()
    {
        skinController = GetComponent<SkinController>();
    }
    // Update is called once per frame

    public void StartEvent(List<string> type)
    {
        if (type == null)
            return;

        foreach (string t in type)
        {
            switch (t?.Trim().ToLowerInvariant())
            {
                case "attack":
                    skinController.Attack();
                    break;
                case "jump":
                    skinController.Jump();
                    break;
            }
        }
    }
}
