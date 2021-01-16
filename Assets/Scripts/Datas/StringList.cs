using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringList", menuName = "ScriptableObjects/StringList")]
public class StringList : ScriptableObject
{
    public List<string> list = new List<string>();
}
