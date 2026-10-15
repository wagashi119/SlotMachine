using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SlotManager : MonoBehaviour
{
    enum SlotState
    {
        Idle,
        Rolling,
        Stopping,
        Stopped,
    }

    [SerializeField] SlotRandomSelect symbolSelector;
    [SerializeField] SlotView slotView;
    [SerializeField] SlotEffect slotEffect;
    [SerializeField] ScoreBoardGenerator scoreBoardGenerator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip rollAudioClip;
    [SerializeField] List<Reels> reels = new List<Reels>();
    [SerializeField] SlotState slotState = SlotState.Idle;
    Reels currentReels;

    public static Reels LastUsedReels { get; private set; }

    //仮追加部分
    [SerializeField] Text remainingAttemptstxt;
    [SerializeField] int maxAttemptstxt = 10;
    [SerializeField] int suripriseRoll = 3;
    [SerializeField] int remainingAttenpts = 0;

    public void Awake()
    {
        slotState = SlotState.Idle;
        if (slotView == null || slotEffect == null || symbolSelector == null || reels.Count == 0 || reels[0] == null)
        {
            Debug.LogError("SlotManager: SlotView, SlotEffect, SlotRandomSelect is not assigned");
            enabled = false;
            return;
        }
        SetCurrentReels(reels[0]);

    }

    public void Roll()
    {
        if (slotState != SlotState.Idle)
        {
            Debug.LogWarning("SlotManager: Slot is not idle");
            return;
        }
        symbolSelector.Roll();
        if (audioSource != null && rollAudioClip != null)
        {
            audioSource.PlayOneShot(rollAudioClip);
        }
        slotState = SlotState.Rolling;

        //仮追加部分
        remainingAttenpts++;
    }

    public void SurpriseSlotChange()
    {
        // サプライズロジックを起動
        SetCurrentReels(reels[1]);
    }

    void Update()
    {
        // スロット起動後かつ、ロール終了後
        if (slotState == SlotState.Rolling && !slotView.IsRolling)
        {
            slotEffect.EffectStart();
            //仮でStartからResultまで動くために以下に色々書いておくのでいらなかったら消しておいてください
            remainingAttemptstxt.text = "残り" + (maxAttemptstxt - remainingAttenpts).ToString() + "回"; 
            slotState = SlotState.Stopping;
        }
        if (slotState == SlotState.Stopping && !slotEffect.Effecting)
        {
            slotState = SlotState.Stopped;
            Debug.Log("SlotManager: Slot is stopped");

            // スコアを計算、追加
            // ロール回数を計算して必要に応じてリールを変更
            if (remainingAttenpts == suripriseRoll)
            {
                SurpriseSlotChange();
            }

            slotState = SlotState.Idle;
            if (remainingAttenpts >= maxAttemptstxt)
            {
                SceneManager.LoadScene("Result");
            }
        }

    }

    void SetCurrentReels(Reels nextReels)
    {
        if (nextReels == null)
        {
            Debug.LogError("SlotManager: Next Reels is null");
            return;
        }

        LastUsedReels = currentReels;
        currentReels = nextReels;
        symbolSelector.ChangeReels(currentReels);
        scoreBoardGenerator.Generate(currentReels);
    }
}
