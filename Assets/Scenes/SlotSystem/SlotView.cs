using UnityEngine;
using UnityEngine.UI;

/**
SlotRamdomSelectから、GetSelectSymbolを入手

*/

public class SlotView : MonoBehaviour
{
    [SerializeField] SlotRandomSelect symbolSelector;
    [SerializeField] int verticalView = 3;
    [SerializeField] Vector2 merign = Vector2.one;

    [SerializeField] Image[] slotImages;

    [SerializeField] float rollTime = 4;
    [SerializeField] float rollStartVelo = 10;
    [SerializeField] AnimationCurve rollVeloCurve = new AnimationCurve();

    bool isRolling;
    float rollTimer;
    int[] targetIndices;
    float[] scrollIndex;
    int[] startIndices;

    void Start()
    {
        int reelCount = symbolSelector.slotsCount;
        int expectedCount = reelCount * verticalView;
        if (slotImages.Length != reelCount && slotImages.Length != expectedCount)
        {
            Debug.LogError("登録されたImageコンポーネント数とスロット数が合いません");
            return;
        }

        symbolSelector.Roll();
        ApplyImages(symbolSelector.GetSelectSymbolIndex());
    }

    private void Update()
    {
        if (!isRolling) return;

        rollTimer += Time.deltaTime;
        float t = Mathf.Clamp01(rollTimer / rollTime);
        float speed = rollStartVelo * rollVeloCurve.Evaluate(t);

        for (int reel = 0; reel < symbolSelector.slotsCount; reel++)
        {
            scrollIndex[reel] += speed * Time.deltaTime;
            int len = symbolSelector.symbols[reel].Count;
            if (scrollIndex[reel] >= startIndices[reel] + len * 2)
            {
                scrollIndex[reel] = targetIndices[reel];
            }
        }

        ApplyScrollImages();

        if (rollTimer >= rollTime)
        {
            isRolling = false;
            ApplyImages(targetIndices);
        }
    }

    /// UnityEventで呼び出される。スロットの回転の初期設定を行うメソッド
    public void CallRoll()
    {
        targetIndices = symbolSelector.GetSelectSymbolIndex();

        int reelCount = symbolSelector.slotsCount;
        scrollIndex = new float[reelCount];
        startIndices = new int[reelCount];

        for (int reel = 0; reel < reelCount; reel++)
        {
            int len = symbolSelector.symbols[reel].Count;
            startIndices[reel] = targetIndices[reel];
            scrollIndex[reel] = startIndices[reel] - len;
        }

        rollTimer = 0f;
        isRolling = true;
    }

    void ApplyScrollImages()
    {
        int reelCount = symbolSelector.slotsCount;
        bool onePerReel = slotImages.Length == reelCount;

        for (int reel = 0; reel < reelCount; reel++)
        {
            int centerIndex = Mathf.FloorToInt(scrollIndex[reel]);

            if (onePerReel)
            {
                SetImageSprite(reel, reel, centerIndex);
                continue;
            }

            for (int row = 0; row < verticalView; row++)
            {
                int imageIndex = reel * verticalView + row;
                if (imageIndex >= slotImages.Length) continue;

                int offset = row - verticalView / 2;
                SetImageSprite(imageIndex, reel, centerIndex + offset);
            }
        }
    }

    void ApplyImages(int[] selectIndices)
    {
        int reelCount = symbolSelector.slotsCount;
        bool onePerReel = slotImages.Length == reelCount;

        for (int reel = 0; reel < reelCount; reel++)
        {
            if (onePerReel)
            {
                SetImageSprite(reel, reel, selectIndices[reel]);
                continue;
            }

            for (int row = 0; row < verticalView; row++)
            {
                int imageIndex = reel * verticalView + row;
                if (imageIndex >= slotImages.Length) continue;

                int offset = row - verticalView / 2;
                SetImageSprite(imageIndex, reel, selectIndices[reel] + offset);
            }
        }
    }

    void SetImageSprite(int imageIndex, int reelIndex, int symbolIndex)
    {
        if (!slotImages[imageIndex]) return;

        var reel = symbolSelector.symbols[reelIndex];
        if (reel == null || reel.Count == 0) return;

        symbolIndex = ((symbolIndex % reel.Count) + reel.Count) % reel.Count;
        slotImages[imageIndex].sprite = reel[symbolIndex].image;
    }
}
