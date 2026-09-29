using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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
    Reels currentReels;

    public static Reels LastUsedReels { get; private set; }
    public Reels CurrentReels => currentReels;

    //仮追加部分
    [SerializeField] Text remainingAttemptstxt;
    int RemainingAttenpts { get { return remainingAttenpts; } set { 
            remainingAttenpts = value;
            if (remainingAttemptstxt != null)
            {
                remainingAttemptstxt.text = $"残り{remainingAttenpts}回";
            }
        } 
    }
    int remainingAttenpts = 10;

    public void Awake() {
        slotState = SlotState.Idle;
        if (slotView == null || slotEffect == null || symbolSelector == null || reels.Count == 0 || reels[0] == null) {
            Debug.LogError("SlotManager: SlotView, SlotEffect, SlotRandomSelect is not assigned");
            enabled = false;
            return;
        }
        SetCurrentReels(reels[0]);

    }

    private void Start()
    {
        //仮追加部分
        //remainingAttemptstxt = GetComponent<Text>();

        RemainingAttenpts = 10;
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

        //仮追加部分
        RemainingAttenpts--;
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

        //仮でStartからResultまで動くために以下に色々書いておくのでいらなかったら消しておいてください
        //remainingAttemptstxt.text = "残り回数" + remainingAttenpts.ToString(); 
        if (RemainingAttenpts <= 0 && slotState == SlotState.Idle)
        {
            SceneManager.LoadScene("Result");

        }

    }

    void ChangeReels() {
        Debug.Log("Change_Reels");
        int reelIndex = rollCount / maxRollCount;
        if (reelIndex >= reels.Count) {
            reelIndex = Mathf.Min(reelIndex, reels.Count - 1);
        }

        SetCurrentReels(reels[reelIndex]);
    }

    void SetCurrentReels(Reels nextReels) {
        currentReels = nextReels;
        LastUsedReels = nextReels;
        symbolSelector.ChangeReels(nextReels);
    }
}
