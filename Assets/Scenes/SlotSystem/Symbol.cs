using UnityEngine;

[CreateAssetMenu(fileName = "Symbol", menuName = "Symbol")]
public class Symbol : ScriptableObject
{
    public Sprite image;
    public GameObject effect; //nullの場合アリ
    [Min(0)] public int score = 3;
    [Min(0f)] public float effectInterval = 0f; //演出の間隔
}
