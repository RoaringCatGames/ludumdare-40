using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundColorSelectionComponent : MonoBehaviour {

  void Awake() {
    SpriteRenderer r = GetComponent<SpriteRenderer>();
    r.color = GameStateManager.instance.GetColor();
  }
}