using System;
using UnityEngine;

public class GlobalActionsComponent : MonoBehaviour {

  public void ResetGame() {
    BranchManager.instance.ResetScene();
  }

  public void PlaySoundEffectByName(string sfxName){
    SoundManager.instance.PlaySfxByName(sfxName);
  }

  public void SetTreeType(string treeType) {
    GameStateManager.instance.TreeTypeKey = this._extractTreeTypeKey(treeType);
  }

  private TreeTypeKey _extractTreeTypeKey(string keyName) {
    switch(keyName){
      case "sakura":
        return TreeTypeKey.SAKURA;
      case "apricot":
        return TreeTypeKey.APRICOT;
      default:
        return TreeTypeKey.SAKURA;
    }
  }
}