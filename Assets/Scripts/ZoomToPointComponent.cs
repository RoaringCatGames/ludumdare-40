using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitterBox.Utils;

public class ZoomToPointComponent : MonoBehaviour {

	public Vector3 targetPoint;
	
	public float pauseTime = 0.5f;
	public float timeToZoom = 0.25f;

	private float originalSize;
	private float targetSize;
	private float elapsedTime = 0f;
	private Vector3 originalPosition;
	private bool isZoomingTo = true;
	private bool isPausing = false;

	// Use this for initialization
	void Start () {
		originalPosition = Camera.main.transform.position;
		targetPoint.z = originalPosition.z;
		originalSize = Camera.main.orthographicSize;		
		targetSize = originalSize/2f;
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += TimeUtils.ThrottledDelta(1f/30f);
		if(isPausing){
			if(elapsedTime >= pauseTime){
				elapsedTime = 0f;
				isPausing = false;
			}
		}else if(isZoomingTo){
			float position = elapsedTime/timeToZoom;
			Camera.main.transform.position = Vector3.Lerp(originalPosition, targetPoint, position);
			Camera.main.orthographicSize = Mathf.Lerp(originalSize, targetSize, position);
			if(elapsedTime >= timeToZoom){
				isZoomingTo = false;
				isPausing = true;
				elapsedTime = 0f;
			}
		}else{
			float position = elapsedTime/(2f*timeToZoom);
			Camera.main.transform.position = Vector3.Lerp(targetPoint, originalPosition, position);
			Camera.main.orthographicSize = Mathf.Lerp(targetSize, originalSize, position);
			if(elapsedTime >= (2f*timeToZoom)){
				Destroy(this);
			}
		}

	}
}
