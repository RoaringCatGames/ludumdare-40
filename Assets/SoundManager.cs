using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	public AudioClip backgroundMusic;
	public AudioClip[] ambientSfx;
	public AudioClip[] bloops;

	public AudioSource music;
	public AudioSource sfx;

	// Use this for initialization
	void Start () {

		if(instance == null){
			instance = this;
			sfx.loop = false;
			PlayBackgroundMusic();
		}else if(instance != this){
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	
	public void PlayBackgroundMusic(){
		music.clip = backgroundMusic;
		music.Play();
	}

	public void PlayBackgroundSfx(int sfxPosition){
		sfx.clip = ambientSfx[sfxPosition];
		sfx.Play();
	}

	public void PlayRandomBloopSfx(){
		sfx.PlayOneShot(bloops[Random.Range(0, bloops.Length)], 0.25f);
	}
}
