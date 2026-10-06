using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipButton : MonoBehaviour
{
    public void Skip()
    {
        Debug.Log("SKIP‚ð‰Ÿ‚µ‚Ü‚µ‚½");

        SceneManager.LoadScene("SampleScene");
    }
}