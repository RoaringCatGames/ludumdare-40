using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyAudioClipPair {
	public string key;
	public AudioClip clip;
}

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	public AudioClip backgroundMusic;
	public AudioClip[] ambientSfx;
	public AudioClip[] bloops;

	public List<KeyAudioClipPair> soundEffects;

	public AudioSource music;
	public AudioSource sfx;

	private Dictionary<string, AudioClip> sfxMap;
	// Use this for initialization
	void Start () {

		if(instance == null){
			instance = this;
			sfx.loop = false;
			// Inititialze our audio clip map so we can
			//	access them quickly.
			sfxMap = new Dictionary<string, AudioClip>();
			foreach(KeyAudioClipPair kacp in soundEffects){
				sfxMap.Add(kacp.key.ToLower(), kacp.clip);
			}

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

	public void PlaySfxByName(string sfxKey){
		sfx.PlayOneShot(sfxMap[sfxKey.ToLower()], 1.0f); 
	}
}
