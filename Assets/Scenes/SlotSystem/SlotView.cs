using UnityEngine;
using UnityEngine.UI;

/**
SlotRamdomSelect����AGetSelectSymbol�����

reelRoots[0] = ���[�AreelRoots[reelCount-1] = �E�[�B
��]�͉E�[���珇�ɊJ�n���A�E�[���珇�ɒ�~����B
*/

public class SlotView : MonoBehaviour
{
    [SerializeField] SlotRandomSelect symbolSelector;

    [SerializeField] Image[] reelRoots;

    [SerializeField] float rollTime = 4f;
    [SerializeField] float rollStartVelo = 10f;
    [SerializeField] AnimationCurve rollVeloCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

    [Header("�E�[���珇�ɂ��炷����")]
    [SerializeField] float reelStartStaggerDelay = 0.25f;
    [SerializeField] float reelStopStaggerDelay = 0.25f;

    [Header("�c�X�N���[��")]
    [Tooltip("0 �Ȃ�q Image �̏����z�u���玩���v�Z")]
    [SerializeField] float rowSpacingOverride;

    Image[,] slotImages;
    Vector2[,] baseAnchoredPositions;
    float rowSpacing;
    int reelCount;
    int verticalView;
    int centerRow;

    bool isRolling;
    public bool IsRolling => isRolling;
    float rollTimer;
    int[] targetIndices;
    float[] scrollIndex;
    int[] startIndices;
    bool[] reelStopped;

    void Start()
    {
        reelCount = symbolSelector.slotsCount;
        if (reelRoots == null || reelRoots.Length != reelCount)
        {
            Debug.LogError($"SlotView: reelRoots �̐�({reelRoots?.Length ?? 0})�ƃX���b�g��({reelCount})����v���Ă��܂���B");
            enabled = false;
            return;
        }

        verticalView = CountChildImages(reelRoots[0]);
        centerRow = verticalView / 2;
        slotImages = new Image[reelCount, verticalView];
        baseAnchoredPositions = new Vector2[reelCount, verticalView];

        for (int reel = 0; reel < reelCount; reel++)
        {
            CollectReelImages(reel);
        }

        CacheBasePositions();
        rowSpacing = ResolveRowSpacing();
    }

    int CountChildImages(Image root)
    {
        int count = 0;
        foreach (var img in root.GetComponentsInChildren<Image>(true))
        {
            if (img == null || img.gameObject == root.gameObject) continue;
            count++;
        }
        return count == 0 ? 1 : count;
    }

    void CollectReelImages(int reel)
    {
        var imgs = reelRoots[reel].GetComponentsInChildren<Image>(true);
        int row = 0;
        foreach (var img in imgs)
        {
            if (img == null) continue;
            if (img.gameObject == reelRoots[reel].gameObject) continue;

            if (row < verticalView)
            {
                slotImages[reel, row] = img;
                row++;
            }
        }

        if (row == 0 && verticalView == 1)
        {
            slotImages[reel, 0] = reelRoots[reel];
        }
    }

    float ResolveRowSpacing()
    {
        if (rowSpacingOverride > 0f) return rowSpacingOverride;

        if (verticalView >= 2)
        {
            return Mathf.Abs(baseAnchoredPositions[0, 1].y - baseAnchoredPositions[0, 0].y);
        }

        var rt = slotImages[0, 0].rectTransform;
        return rt.rect.height > 0f ? rt.rect.height : 100f;
    }

    void CacheBasePositions()
    {
        for (int reel = 0; reel < reelCount; reel++)
        {
            for (int row = 0; row < verticalView; row++)
            {
                var rt = slotImages[reel, row].rectTransform;
                baseAnchoredPositions[reel, row] = rt.anchoredPosition;
            }
        }
    }

    void Update()
    {
        if (!isRolling) return;

        rollTimer += Time.deltaTime;
        bool allStopped = true;

        for (int reel = 0; reel < reelCount; reel++)
        {
            float startAt = GetReelStartTime(reel);
            float stopAt = GetReelStopTime(reel);

            if (rollTimer < startAt)
            {
                allStopped = false;
                continue;
            }

            if (reelStopped[reel])
            {
                ApplyReelStopped(reel);
                continue;
            }

            allStopped = false;

            float localTime = rollTimer - startAt;
            float t = Mathf.Clamp01(localTime / rollTime);
            float speed = rollStartVelo * rollVeloCurve.Evaluate(t);

            scrollIndex[reel] += speed * Time.deltaTime;

            int len = symbolSelector.symbols[reel].Count;
            if (scrollIndex[reel] >= startIndices[reel] + len * 2 || rollTimer >= stopAt)
            {
                scrollIndex[reel] = targetIndices[reel];
                reelStopped[reel] = true;
                ApplyReelStopped(reel);
                continue;
            }

            ApplyReelScrolling(reel);
        }

        if (allStopped)
        {
            isRolling = false;
        }
    }

    public void RollStart()
    {
        targetIndices = symbolSelector.GetSelectSymbolIndex();

        // eorror : IndexOutOfRangeException: Index was outside the bounds of the array.
        int arrayLength = targetIndices.Length;
        scrollIndex = new float[arrayLength];
        startIndices = new int[arrayLength];
        reelStopped = new bool[arrayLength];

        for (int reel = 0; reel < reelCount; reel++)
        {
            int len = symbolSelector.symbols[reel].Count;
            startIndices[reel] = targetIndices[reel];
            scrollIndex[reel] = startIndices[reel] - len;
            reelStopped[reel] = false;
        }

        rollTimer = 0f;
        isRolling = true;
    }

    int OrderFromRight(int reel) => reelCount - 1 - reel;

    float GetReelStartTime(int reel) => OrderFromRight(reel) * reelStartStaggerDelay;

    float GetReelStopTime(int reel) => rollTime + OrderFromRight(reel) * reelStopStaggerDelay;

    void ApplyReelScrolling(int reel)
    {
        float fraction = scrollIndex[reel] - Mathf.Floor(scrollIndex[reel]);
        int centerIndex = Mathf.FloorToInt(scrollIndex[reel]);
        float yOffset = -fraction * rowSpacing;

        for (int row = 0; row < verticalView; row++)
        {
            var rt = slotImages[reel, row].rectTransform;
            rt.anchoredPosition = baseAnchoredPositions[reel, row] + new Vector2(0f, yOffset);

            int symbolIndex = centerIndex + row - centerRow;
            SetImageSprite(reel, row, symbolIndex);
        }
    }

    void ApplyReelStopped(int reel)
    {
        ResetReelPositions(reel);
        ApplyReelImages(reel, targetIndices[reel]);
    }

    void ResetReelPositions(int reel)
    {
        for (int row = 0; row < verticalView; row++)
        {
            slotImages[reel, row].rectTransform.anchoredPosition = baseAnchoredPositions[reel, row];
        }
    }

    void ApplyReelImages(int reel, int centerIndex)
    {
        for (int row = 0; row < verticalView; row++)
        {
            int offset = row - centerRow;
            SetImageSprite(reel, row, centerIndex + offset);
        }
    }

    void ApplyImages(int[] selectIndices)
    {
        for (int reel = 0; reel < reelCount; reel++)
        {
            ResetReelPositions(reel);
            ApplyReelImages(reel, selectIndices[reel]);
        }
    }

    void SetImageSprite(int reelIndex, int rowIndex, int symbolIndex)
    {
        Image img = slotImages[reelIndex, rowIndex];
        if (!img) return;

        var reel = symbolSelector.symbols[reelIndex];
        if (reel == null || reel.Count == 0) return;

        symbolIndex = ((symbolIndex % reel.Count) + reel.Count) % reel.Count;
        img.sprite = symbolSelector.GetSymbol(new(reelIndex, symbolIndex)).image;
    }
}
