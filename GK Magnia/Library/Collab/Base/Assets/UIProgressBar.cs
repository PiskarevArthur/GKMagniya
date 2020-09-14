using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProgressBar : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI T;
    [SerializeField] private RectTransform progress;
    private void Awake()
    {
        GameManager.EventChangeCurrentLevel += OnChange;
        OnChange();
    }
    private void OnChange()
    {
        T.text = (GameManager.currentLevel + 1).ToString();
    }
    private void OnDestroy()
    {
        GameManager.EventChangeCurrentLevel -= OnChange;

    }

    void Update()
    {
        progress.localScale = Vector3.Lerp(progress.localScale,
            new Vector3( 1f-(GameManager.MyTimer/ GameManager.LevelGameDesign.Stars[GameManager.LevelGameDesign.Stars.Length - 1]), progress.localScale.y, progress.localScale.z),
            Time.deltaTime*2f);
    }
}
