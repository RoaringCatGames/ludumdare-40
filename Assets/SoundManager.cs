using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	public AudioClip backgroundMusic;
	public AudioClip[] ambientSfx;
	public AudioSource music;
	public AudioSource sfx;

	private float elapsedTime = 0f;
	private float pauseTime = 2f;
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


	void Update(){
		// if(!sfx.isPlaying) {
		// 	elapsedTime += Time.deltaTime;

		// 	if(elapsedTime >= pauseTime){
		// 		pauseTime = Random.Range(1.5f, 5f);
		// 		PlayBackgroundSfx(Random.Range(0, ambientSfx.Length));
		// 	}
		// }
	}

	
	public void PlayBackgroundMusic(){
		music.clip = backgroundMusic;
		music.Play();
	}

	public void PlayBackgroundSfx(int sfxPosition){
		sfx.clip = ambientSfx[sfxPosition];
		sfx.Play();
	}
}
