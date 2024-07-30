using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LabelsList", menuName = "Configs/LabelsList")]
public class LabelsList : ScriptableObject
{
    public List<string> labels = new List<string>();
}