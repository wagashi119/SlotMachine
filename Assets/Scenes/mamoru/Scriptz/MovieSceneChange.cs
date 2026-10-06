
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MovieSceneChange : MonoBehaviour
{
    [SerializeField] VideoPlayer movie;

    private bool sceneChanged = false;

    void Start()
    {
        if (movie != null)
        {
            movie.loopPointReached += OnMovieFinished;
        }
        else
        {
            Debug.LogError("VideoPlayerÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
        }
    }

    void OnMovieFinished(VideoPlayer vp)
    {
        ChangeScene();
    }

    public void ChangeScene()
    {
        if (sceneChanged) return;

        sceneChanged = true;
        SceneManager.LoadScene("SampleScene");
    }

    private void OnDestroy()
    {
        if (movie != null)
        {
            movie.loopPointReached -= OnMovieFinished;
        }
    }
}