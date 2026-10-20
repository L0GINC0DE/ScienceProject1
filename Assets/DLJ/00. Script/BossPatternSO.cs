using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BossPatternSO", menuName = "Scriptable Objects/BossPatternSO")]
public class BossPatternSO : ScriptableObject
{
    public AbstractPatton patternPrefab;

    public Vector2 areaSize;

    public float duration = 5f;
}
