using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedAnimationComponent : MonoBehaviour {

	public string animationName;
	public float delaySeconds = 0f;

	private float elapsedTime = 0f;
	private Animator animator;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += TimeUtils.ThrottledDelta(1f/30f);

		if(elapsedTime >= delaySeconds){
			if(animator != null){
				transform.localScale = new Vector3(1f, 1f, 1f);
				animator.Play(animationName);
				if(Random.Range(0f, 1f) >= 0.75f){
					SoundManager.instance.PlayRandomBloopSfx();
				}
			}

			Destroy(this);
		}
	}
}
