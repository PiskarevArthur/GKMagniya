using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelNumber : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI T;
    private void Awake()
    {
        GameManager.EventChangeCurrentLevel += OnChange;
        OnChange();
    }
    private void OnChange()
    {
        T.text = (GameManager.currentLevel +1).ToString();
    }
    private void OnDestroy()
    {
        GameManager.EventChangeCurrentLevel -= OnChange;

    }
}
