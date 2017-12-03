using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameControlsController : MonoBehaviour {

	private Animator animator;

	private bool areControlsShowing = false;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(BranchManager.instance.IsGameOver() && !areControlsShowing){
			Logger.Log("GAME ENDED");
			areControlsShowing = true;
			animator.Play("end-game-controls-enter");
		}
	}

	public void HideControls(){
		animator.Play("end-game-controls-leave");
		areControlsShowing = false;
	}
}
