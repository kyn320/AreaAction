using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;

    AudioSource[] audios;

    public AudioClip[] voices;
    public AudioClip uiSE;

    void Awake() {
        instance = this;
    }

	// Use this for initialization
	void Start () {
        audios = GetComponents<AudioSource>();
	}


    public void PlayComboVoice(int count) {
        switch (count) {
            case 5:   audios[1].PlayOneShot(voices[0],1F);   break;
            case 10:   audios[1].PlayOneShot(voices[1],1F);   break;
            case 15:   audios[1].PlayOneShot(voices[2],1F);   break;
            case 20:  audios[1].PlayOneShot(voices[3],1F);   break;
            case 25:  audios[1].PlayOneShot(voices[4],1F);   break;
            case 30: audios[1].PlayOneShot(voices[5], 1F); break;
            default:
                if (count > 30 && count % 5 == 0) {
                    audios[1].PlayOneShot(voices[5], 1F);
                }
                break;
        }

    }

    public void PlayUISE() {
        audios[1].PlayOneShot(uiSE,1f);
    }

}
