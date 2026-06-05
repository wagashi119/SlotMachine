using UnityEngine;

public class SlotEffect : MonoBehaviour
{
    [SerializeField] SlotScoreManager scoreManager;
    [SerializeField] Transform effectRoot;

    [SerializeField] SlotScore[] slotScores;
    [SerializeField] bool effecting = false;
    [SerializeField] float effectInterval = 1f;
    float effectTimer = 0;
    [SerializeField] float symboleffectInterbal = 0.5f;

    public bool Effecting => effecting;

    public void EffectStart() {
        Debug.Log("SlotEffect: EffectStart");   
        effecting = true;
        Invoke("EffectEnd", 2f);
    }

    void Update() {
        if (!effecting) return;
    }

    void EffectEnd() {
        Debug.Log("SlotEffect: EffectEnd");
        effecting = false;
    }
}
