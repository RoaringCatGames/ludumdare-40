using System;
using UnityEngine;

public class LineSegment {

  public LineSegment(){
    StartPoint = new Vector3();
    EndPoint = new Vector3();
  }
  public LineSegment(Vector3 start, Vector3 end){
    StartPoint = start;
    EndPoint = end;
  }

  public LineSegment(float startX, float startY, float endX, float endY){
    StartPoint = new Vector3(startX, startY);
    EndPoint = new Vector3(endX, endY);
  }

  public Vector3 StartPoint {get; set;}
  public Vector3 EndPoint {get; set;}

  public override string ToString(){
    return StartPoint + " -> " + EndPoint;
  }
}