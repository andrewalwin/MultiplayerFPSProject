using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    private Queue<AudioSource> audioSourceQueue = new Queue<AudioSource>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Debug.Log("AudioManager already exists");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update () {
		
	}

    //GameObject requests the AudioManager to play a clip for them in a specific location

    //GameObject requests the AudioManager to play a clip globally for them
}
