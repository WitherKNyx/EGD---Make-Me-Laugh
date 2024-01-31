using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(5, 10)]
    public string text;
    [Tooltip("A list of sprites to be displayed in the dialogue. Make sure all indices used in the above text are present in this list.")]
    public List<Sprite> sprites;
}
