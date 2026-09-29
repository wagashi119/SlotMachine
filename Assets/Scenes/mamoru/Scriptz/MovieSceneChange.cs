using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class MovieSceneChange : MonoBehaviour
{
    public VideoPlayer _movie;
    private float movieTime = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movieTime = (float)_movie.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        movieTime -= Time.deltaTime;

        if(movieTime < 0)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
