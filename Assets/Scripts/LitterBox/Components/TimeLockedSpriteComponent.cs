using System;
using UnityEngine;

namespace LitterBox.Components {

  [RequireComponent(typeof(SpriteRenderer))]
  public class TimeLockedSpriteComponent : MonoBehaviour {


    [Tooltip("X = Year, Y = Month, Z = Year")]
    public Vector3 startTime;
    [Tooltip("X = Year, Y = Month, Z = Year")]
    public Vector3 endTime;

    public Sprite spriteDuringTimeSpan;
    public Sprite defaultSprite;

    void Awake() {
      SpriteRenderer r = GetComponent<SpriteRenderer>();
      var now = DateTime.Now;
      var beginning = new DateTime((int)startTime.x, (int)startTime.y, (int)startTime.z);
      var end = new DateTime((int)endTime.x, (int)endTime.y, (int)endTime.z);

      if(beginning <= now && now <= end) {
        r.sprite = spriteDuringTimeSpan;
      }else{
        r.sprite = defaultSprite;
      }
    }
  }
}