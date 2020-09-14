using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Level : ScriptableObject
{
    [SerializeField] public int[] Stars = { 10, 20, 999 };
    [SerializeField] public float ExplodeForce=1;

}
