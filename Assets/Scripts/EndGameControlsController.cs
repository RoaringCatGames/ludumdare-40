using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitterBox.Utils;

public class EndGameControlsController : MonoBehaviour {

	private Animator animator;

	private bool areControlsShowing = false;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Using Late Update is a total Hack here, to try and time the 
	//	controlsShowing flag from getting reset
	void LateUpdate () {
		if(BranchManager.instance.IsGameOver() && !areControlsShowing){
			ShowControls();
		} else if(!BranchManager.instance.IsGameOver() && areControlsShowing) {
			HideControls();
		}
	}

	public void HideControls(){
		animator.Play("end-game-controls-leave");
		areControlsShowing = false;
	}

	public void ShowControls() {
		animator.Play("end-game-controls-enter");
		areControlsShowing = true;
	}

	public void ToggleControls(bool shouldShow){
		if(shouldShow) {
			this.ShowControls();
		}else{
			this.HideControls();
		}

	}
}
