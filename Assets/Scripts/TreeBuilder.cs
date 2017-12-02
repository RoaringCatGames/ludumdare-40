using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeBuilder : MonoBehaviour {

	[Header("Growth Properties")]
	public float secondsBetweenSegments = 2f;
	public float maxHeight = 20f;
	public float maxLeftRightDistance = 10f;
	public Vector2 xMinMaxChangeMagnitude = new Vector2(0.1f, 1f);
	public Vector2 yMinMaxChangeMagnitude = new Vector2(0.3f, 1.5f);
	public float maxBaseWidth = 3f;
	public float tipWidth = 0.25f;


	[HideInInspector]
	public List<LineSegment> LineSegments = new List<LineSegment>();

	private LineRenderer lineRenderer;
	private float elapsedTime = 0f;
	private Vector3 nextTargetPosition;

	// Use this for initialization
	void Awake () {
		lineRenderer = GetComponent<LineRenderer>();

		if(lineRenderer){
			Vector3[] positions = new Vector3[] {
				new Vector3(0, 0, 0),
				new Vector3(0, 1, 0)
			};

			lineRenderer.widthMultiplier = 1f;  		
			lineRenderer.positionCount = positions.Length;
			lineRenderer.SetPositions(positions);

			LineSegments.Add(new LineSegment(positions[0], positions[1]));
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Throttle updates to 60fps
		float throttledDeltaTime = Mathf.Clamp(Time.deltaTime, 0, 1f/60f);

	
		Vector3 topPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
		Vector3 prevPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
		Vector3[] linePositions;
		Vector3 newTopPosition;
		int linePositionsSize = lineRenderer.positionCount;

		if(nextTargetPosition == null || topPosition.x == nextTargetPosition.x && topPosition.y == nextTargetPosition.y) {
			nextTargetPosition = generateNextPoint(topPosition);
			//Add a new position to our linePositions
			linePositionsSize += 1;			
			//Duplicate Top Position
			newTopPosition = topPosition;
			LineSegments.Add(new LineSegment(topPosition, newTopPosition));

			//TODO: Update animation Curve based on size
			elapsedTime = 0f;
		}else{
			// Lerp to Target!!
			elapsedTime += throttledDeltaTime;
			float positionOfGrowthTime = elapsedTime/secondsBetweenSegments;
			newTopPosition = Vector3.Lerp(prevPosition, nextTargetPosition, positionOfGrowthTime);
			
			//Update our last Line Segment
			LineSegments.Last().EndPoint = newTopPosition;						
		}

		//Reset Positions
		linePositions = new Vector3[linePositionsSize];
		lineRenderer.GetPositions(linePositions);
		linePositions[linePositions.Length - 1] = newTopPosition;
		lineRenderer.positionCount = linePositions.Length;
		float baseWidth = Mathf.Clamp((Vector3.Distance(newTopPosition, linePositions[0])/maxHeight) * maxBaseWidth, tipWidth, maxBaseWidth);
		lineRenderer.widthCurve = new AnimationCurve(
			new Keyframe(0f, baseWidth),
			new Keyframe(1f, tipWidth)
		);
		lineRenderer.SetPositions(linePositions);	
	}

	private Vector3 generateNextPoint(Vector3 lastPoint){
		float nextX, nextY;
		int direction = Random.Range(0f, 1f) >= 0.5f ? 1 : -1;
		nextX = Random.Range(direction * xMinMaxChangeMagnitude.x, direction * xMinMaxChangeMagnitude.y) + lastPoint.x;
		nextX = Mathf.Clamp(nextX, -maxLeftRightDistance, maxLeftRightDistance);
		nextY = Random.Range(yMinMaxChangeMagnitude.x, yMinMaxChangeMagnitude.y) + lastPoint.y;
		nextY = Mathf.Clamp(nextY, 0f, maxHeight);		
		return new Vector3(
			nextX,
			nextY,
			0f
		);
	}
}
