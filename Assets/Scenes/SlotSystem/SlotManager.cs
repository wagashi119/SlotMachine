using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    enum SlotState {
        Idle,
        Rolling,
        Stopping,
        Stopped,
    }

    [SerializeField] SlotRandomSelect symbolSelector;
    [SerializeField] SlotView slotView;
    [SerializeField] SlotEffect slotEffect;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip rollAudioClip;
    [SerializeField] int rollCount = 0;
    [SerializeField] int maxRollCount = 10;
    [SerializeField] List<Reels> reels = new List<Reels>();
    [SerializeField] SlotState slotState = SlotState.Idle;

    public void Awake() {
        slotState = SlotState.Idle;
        if (slotView == null || slotEffect == null || symbolSelector == null) {
            Debug.LogError("SlotManager: SlotView, SlotEffect, SlotRandomSelect is not assigned");
            enabled = false;
            return;
        }
        symbolSelector.ChangeReels(reels[0]);
    }

    public void Roll()
    {
        if (slotState != SlotState.Idle) {
            Debug.LogWarning("SlotManager: Slot is not idle");
            return;
        }
        symbolSelector.Roll();
        if (audioSource != null && rollAudioClip != null) {
            audioSource.PlayOneShot(rollAudioClip);
        }
        slotState = SlotState.Rolling;
        rollCount++;
    }

    bool ReelsChangeRoll => rollCount % maxRollCount == 0;
    void Update()
    {
        // スロット起動後かつ、ロール終了後
        if (slotState == SlotState.Rolling && !slotView.IsRolling) {
            slotEffect.EffectStart();
            slotState = SlotState.Stopping;
        }
        if (slotState == SlotState.Stopping && !slotEffect.Effecting) {
            slotState = SlotState.Stopped;
            Debug.Log("SlotManager: Slot is stopped");

            // スコアを計算、追加
            // ロール回数を計算して必要に応じてリールを変更
            if (ReelsChangeRoll) {
                ChangeReels();
            }
            slotState = SlotState.Idle;
        }
    }

    void ChangeReels() {
        Debug.Log("Change_Reels");
        int reelIndex = rollCount / maxRollCount;
        if (reelIndex >= reels.Count) {
            reelIndex = Mathf.Min(reelIndex, reels.Count - 1);
        }

        symbolSelector.ChangeReels(reels[reelIndex]);
    }
}
