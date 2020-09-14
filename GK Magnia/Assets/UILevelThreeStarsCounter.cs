using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelThreeStarsCounter : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI T;
    private void Reset()
    {
        T = GetComponent<TMPro.TextMeshProUGUI>();
    }
    private void Awake()
    {
        GameManager.EventChangeCurrentLevel += OnChange;
        OnChange();
    }
    private void OnChange()
    {
        T.text =(GameManager. LevelGameDesign.Stars[GameManager.LevelGameDesign.Stars.Length - 1]).ToString();
    }

    private void Update()
    {
        if (GameManager.CurrentState == GameManager.Mode.Play) T.text = Mathf.RoundToInt(GameManager.LevelGameDesign.Stars[GameManager.LevelGameDesign.Stars.Length - 1] - GameManager.MyTimer).ToString();
    }
    private void OnDestroy()
    {
        GameManager.EventChangeCurrentLevel -= OnChange;

    }
    
}
