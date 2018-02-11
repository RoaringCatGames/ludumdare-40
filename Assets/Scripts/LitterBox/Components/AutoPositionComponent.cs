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
    public bool positionAfterInitOnly = false;

    private RectTransform rectTransform;
    private bool hasPositioned = false;
    void Start() {
      rectTransform = GetComponent<RectTransform>();
    } 

    void Update() {
      if(!positionAfterInitOnly || (positionAfterInitOnly && !hasPositioned)) {
        float top = Camera.main.transform.position.y + Camera.main.orthographicSize;
        float bottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
        float left = Camera.main.transform.position.x - Camera.main.orthographicSize * Screen.width / Screen.height;
        float right = Camera.main.transform.position.x + Camera.main.orthographicSize * Screen.width / Screen.height;

        float newX, newY;
        switch(target) {
          case AutoPositionTarget.LEFT_CENTER:
            newX = left + (rectTransform.rect.width/2f) + offsetX;
            newY = Camera.main.transform.position.y;
            break;
          case AutoPositionTarget.BOTTOM_LEFT:
            newX = left + (rectTransform.rect.width/2f) + offsetX;
            newY = bottom + (rectTransform.rect.height/2f) + offsetY;
            break;
          default:
            newX = left + (rectTransform.rect.width/2f) + offsetX;
            newY = bottom + (rectTransform.rect.height/2f) + offsetY;
            break;
        }

        rectTransform.position = new Vector3(newX, newY, rectTransform.position.z);
        hasPositioned = true;
      }
    }
  }
}