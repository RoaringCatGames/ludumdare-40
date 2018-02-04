using System;
using UnityEngine;

public class GlobalActionsComponent : MonoBehaviour {

  public void ResetGame() {
    BranchManager.instance.ResetScene();
  }

  public void PlaySoundEffectByName(string sfxName){
    SoundManager.instance.PlaySfxByName(sfxName);
  }
}