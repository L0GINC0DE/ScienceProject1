using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BossPatternSO", menuName = "Scriptable Objects/BossPatternSO")]
public class BossPatternSO : ScriptableObject
{
    [FormerlySerializedAs("abstractPatton")]
    public AbstractPatton patternPrefab;

    public Vector2 areaSize;

    public float duration = 5f;
}
