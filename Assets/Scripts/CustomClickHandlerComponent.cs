using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ClickedEvent : UnityEvent {}

public class CustomClickHandlerComponent : MonoBehaviour {

  public ClickedEvent onClick;

  void Start() {
    if(GetComponent<Collider2D>() == null){
      Debug.LogWarning("CustomClickHandlerComponent Expects a Collider to be on the game object in order to fire a click");
    }
  }

  public void OnMouseDown(){    
    this.onClick.Invoke();
  }
}