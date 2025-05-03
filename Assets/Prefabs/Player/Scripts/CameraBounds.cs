using UnityEngine;

[CreateAssetMenu(fileName = "CameraBounds", menuName = "Scriptable Objects/CameraBounds")]
public class CameraBounds : ScriptableObject
{
    public float LeftBound;
    public float RightBound;
    public float TopBound;
    public float BottomBound;
}
