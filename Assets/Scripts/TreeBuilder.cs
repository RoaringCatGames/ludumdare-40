using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeBuilder : MonoBehaviour {

	public float secondsBetweenSegments = 2f;
	public float maxHeight = 20f;
	public float maxXOffset = 10f;

	public Vector2 xMinMaxChangeMagnitude = new Vector2(0.1f, 1f);
	public Vector2 yMinMaxChangeMagnitude = new Vector2(0.3f, 1.5f);

	private LineRenderer lineRenderer;

	private float elapsedTime = 0f;

	// Use this for initialization
	void Awake () {
		lineRenderer = GetComponent<LineRenderer>();

		if(lineRenderer){
			Vector3[] positions = new Vector3[] {
				new Vector3(0, 0, 0),
				new Vector3(0, 1, 0)
			};

			//lineRenderer.widthMultiplier = 0.1f; 
			lineRenderer.positionCount = positions.Length;
			lineRenderer.SetPositions(positions);
		}
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;
		if(elapsedTime >= secondsBetweenSegments){
			elapsedTime = elapsedTime%secondsBetweenSegments;
			
			//Setup Next Position
			Vector3 lastPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
			Vector3 nextPos = generateNextPoint(lastPosition);	
			
			//Build new Positions Array
			Vector3[] linePositions = new Vector3[lineRenderer.positionCount + 1];
			int positionsReturned = lineRenderer.GetPositions(linePositions);
			linePositions[linePositions.Length - 1] = nextPos;

			//Reset Positions
			lineRenderer.positionCount = linePositions.Length;
			lineRenderer.SetPositions(linePositions);			
		}
	}

	private Vector3 generateNextPoint(Vector3 lastPoint){
		float nextX, nextY;
		int direction = Random.Range(0f, 1f) >= 0.5f ? 1 : -1;
		nextX = Random.Range(direction * xMinMaxChangeMagnitude.x, direction * xMinMaxChangeMagnitude.y) + lastPoint.x;
		nextX = Mathf.Clamp(nextX, -maxXOffset, maxXOffset);
		nextY = Random.Range(yMinMaxChangeMagnitude.x, yMinMaxChangeMagnitude.y) + lastPoint.y;
		nextY = Mathf.Clamp(nextY, 0f, maxHeight);

		Logger.Log("Next X: ",  nextX , "Next Y: ", nextY);
		return new Vector3(
			nextX,
			nextY,
			0f
		);
	}
}
