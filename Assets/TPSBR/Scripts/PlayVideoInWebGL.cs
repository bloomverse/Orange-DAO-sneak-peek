using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class PlayVideoInWebGL : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private string videoFileName;

    public bool initialStatus = true;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL //&& !UNITY_EDITOR
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        //videoPlayer.StepForward();
        videoPlayer.Play();

        if (!initialStatus)
        {
            videoPlayer.Pause();
        }

        //videoPlayer.loopPointReached += EndReached;
        //        Debug.Log(videoPlayer.url);
        ///videoPlayer.Play();
#endif
    }


    //void EndReached(UnityEngine.Video.VideoPlayer vp){
    //    playing = false;
    // }

    void OnTriggerStay(Collider other)
    {
        // Debug.Log("opennign door");
        if (other.gameObject.tag == "Player" && !videoPlayer.isPlaying)
        {
            startPlaying();
        }

    }

    public void startPlaying()
    {
        //        Debug.Log("playing");
        videoPlayer.Play();
    }
}