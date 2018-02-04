using System;
using UnityEngine;
using LitterBox.Utils;

[RequireComponent(typeof(SpriteRenderer))]
public class CalligratreeColorSelectionComponent : MonoBehaviour {

  void Awake() {    
    ColorOptionsComponent colorOptions = GetComponent<ColorOptionsComponent>();
    SpriteRenderer r = GetComponent<SpriteRenderer>();

    if(colorOptions == null) {      
      r.color = GameStateManager.instance.GetColor();
    }else{
      
      r.color = colorOptions.GetColor(GameStateManager.instance.TreeTypeKey);
    }
  }
}