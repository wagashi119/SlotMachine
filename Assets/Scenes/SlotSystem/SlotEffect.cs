using UnityEngine;
using System.Collections.Generic;

public class SlotEffect : MonoBehaviour
{
    [SerializeField] SlotScoreManager scoreManager;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Transform effectRoot;
    [SerializeField] GameObject matchEffectPanel;

    [SerializeField] IReadOnlyList<MatchState> slotScores;
    [SerializeField] bool effecting = false;
    [SerializeField,Min(0.01f)] float matchEffectInterbal = 0.5f;
    [SerializeField,Min(0.01f)] float symbolEffectInterval = 1f;
    [SerializeField] int effectCount = 0;

    public bool Effecting => effecting;

    public void EffectStart() {
        Debug.Log("SlotEffect: EffectStart");
        effecting = true;
        effectCount = 0;
        slotScores = scoreManager.MatchedSymbols;

        Invoke("MatchEffect", matchEffectInterbal);
    }

    // 再起実行
    void MatchEffect() {
        if (slotScores.Count <= effectCount) {
            effecting = false;
            return;
        }
        GameObject effectPrefab = slotScores[effectCount].effect;
        Debug.Log($"MatchEffect:{effectCount}, {effectPrefab?.name ?? "null"}");
        effectCount++;

        // インスタンスが設定されていれば生成
        if (effectPrefab != null) {
            GameObject effectInstance = Instantiate(effectPrefab, effectRoot);
            //Destroy(effectInstance, symbolEffectInterval);
        }
        audioSource.Play();
        
        // 記録された演出回数から、終了タイミングを検知
        if (slotScores.Count <= effectCount) {
            effecting = false;
        } else {
            Invoke("MatchEffect", matchEffectInterbal + slotScores[effectCount].effectInterval);
        }
    }
}
