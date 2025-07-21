using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManualInfo", menuName = "Custom/Manual", order = 2)]
public class ManualItem : ScriptableObject
{
    public Sprite icon;
    public string name;
    public string info;
}
