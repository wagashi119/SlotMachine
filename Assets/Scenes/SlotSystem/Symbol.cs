using UnityEngine;

[CreateAssetMenu(fileName = "Symbol", menuName = "Symbol")]
public class Symbol : ScriptableObject
{
    public Sprite image;
    public GameObject effect; //nullの場合アリ
    public int score = 3;
    
}
