using System;
using UnityEngine;

namespace LitterBox.Components {

  [Serializable]
  public enum AutoPositionTarget {
    TOP_LEFT,
    TOP_CENTER, 
    TOP_RIGHT, 
    LEFT_CENTER,
    CENTER_CENTER, 
    RIGHT_CENTER,
    BOTTOM_LEFT, 
    BOTTOM_CENTER,
    BOTTOM_RIGHT
  }
  [RequireComponent(typeof(RectTransform))]
  public class AutoPositionComponent : MonoBehaviour {

    public AutoPositionTarget target;
    public float offsetX = 0f;
    public float offsetY = 0f;

    private RectTransform rectTransform;
    void Start() {
      rectTransform = GetComponent<RectTransform>();
      
    } 

    void Update() {
      float top = Camera.main.transform.position.y + Camera.main.orthographicSize;
      float bottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
      float left = Camera.main.transform.position.x - Camera.main.orthographicSize * Screen.width / Screen.height;
      float right = Camera.main.transform.position.x + Camera.main.orthographicSize * Screen.width / Screen.height;

      float newX = left + (rectTransform.rect.width/2f) + offsetX;
      float newY = bottom + (rectTransform.rect.height/2f) + offsetY;

      rectTransform.position = new Vector3(newX, newY, rectTransform.position.z);
    }
  }
}