using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelColors : MonoBehaviour
{

    [SerializeField] Color[] LevelColor;
    [SerializeField] MeshRenderer MS;
    private void OnValidate()
    {
        if (MS == null) MS = gameObject. GetComponent<MeshRenderer>();
    }

    private void Awake()
    {
        GameManager.EventChangeCurrentLevel += OnChange;
    }

    private void OnChange()
    {
        MS.sharedMaterial.SetColor("_Color2", LevelColor[GameManager.currentLevel]);
    }
    private void OnDestroy()
    {
        GameManager.EventChangeCurrentLevel -= OnChange;

    }
}
