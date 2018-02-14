using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteOptionsComponent))]
public class CalligratreeSpriteSelectionComponent : MonoBehaviour {

  void Awake() {    
    SpriteOptionsComponent spriteOptions = GetComponent<SpriteOptionsComponent>();
    SpriteRenderer r = GetComponent<SpriteRenderer>();

    if(spriteOptions != null) {   
      r.sprite = spriteOptions.GetSprite(GameStateManager.instance.TreeTypeKey);   
    }
  }
}