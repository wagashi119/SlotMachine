using UnityEngine;
using UnityEngine.UI;

public class ResultPage : MonoBehaviour
{
    [SerializeField] int page;
    [SerializeField] AudioClip flipAudioPlip;

    [Space(3)]
    [SerializeField] Transform mainPosition;
    [SerializeField] Color mainColor = Color.white;

    [Space(3)]
    [SerializeField] Transform offPosition;
    [SerializeField] Color offColor = Color.gray;
    AudioSource audioSource;

    private void Awake()
    {
        page = 0;
        audioSource = GetComponent<AudioSource>();
    }

    [ContextMenu("click")]
    public void PageFlip()
    {
        Debug.Log("flip");

        // Šù‚É‚ß‚­‚ç‚ê‚Ä‚¢‚é‚©
        if (transform.childCount - 1 <= page)
        {
            return;
        }

        //
        Transform backPage = transform.GetChild(page);
        backPage.position = offPosition.position;
        backPage.GetComponent<RawImage>().color = offColor;

        page++;
        if (flipAudioPlip)
        {
            audioSource.PlayOneShot(flipAudioPlip);
        }
        
        // ŠJ‚­GameObject
        Transform nextPage = transform.GetChild(page);
        nextPage.gameObject.SetActive(true);
        nextPage.position = mainPosition.position;
        nextPage.GetComponent<RawImage>().color = mainColor;
    }
}
