using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ClickedEvent : UnityEvent {}

public class CustomClickHandlerComponent : MonoBehaviour {

  public ClickedEvent onClick;

  private Collider2D collider;

  void Start() {
    collider = GetComponent<Collider2D>();

    if(collider == null){
      Debug.LogWarning("CustomClickHandlerComponent Expects a Collider to be on the game object in order to fire a click");
    }
  }

  public void OnMouseDown(){    
    this.onClick.Invoke();
  }
}