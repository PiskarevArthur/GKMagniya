using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCursor : MonoBehaviour
{
    [SerializeField] RectTransform Selected;
    [SerializeField] RectTransform Stick;
    private void Update()
    {
        if (GameManager.LockedObject != null)
        {
           Selected.transform.position=  Camera.main.WorldToScreenPoint(GameManager.LockedObject.MC.bounds.center) ;
        }
    }
}
