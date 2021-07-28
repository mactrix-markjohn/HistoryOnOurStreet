using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using YoutubePlayer;

public class YouTubeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<VideoPlayer>().PlayYoutubeVideoAsync("https://www.youtube.com/watch?v=1PuGuqpHQGo");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
