using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BranchComponent : MonoBehaviour
{
  public static readonly int MAX_BRANCH_DEPTH = 2;
  public static readonly float CHILD_BRANCH_SCALE_FACTOR = 0.5f;
  public static readonly int TRUNK_BASE_SEGMENTS = 6;
  public static readonly int SEGMENTS_BETWEEN_BRANCH_POINTS = 4;

  [Header("Initial Segment")]
  public Vector2 firstPosition = new Vector2(0f, 1.5f);
  public int branchLevel = 0;

  [Header("Growth Properties")]
  public float secondsBetweenSegments = 0.5f;
  public float maxHeight = 20f;
  public float maxLeftRightDistance = 10f;
  public Vector2 xMinMaxChangeMagnitude = new Vector2(0.1f, 1f);
  public Vector2 yMinMaxChangeMagnitude = new Vector2(0.3f, 1.5f);
  public float maxBaseWidth = 3f;
  public float tipWidth = 0.25f;

  [Header("Rendering Settings")]
  public Material branchMaterial;
  public GameObject animatedLeafPrefab;
  public GameObject animatedFlowerPrefab;

  [Header("User Interactoins")]
  public GameObject selectionPointPrefab;


  [HideInInspector]
  public List<LineSegment> LineSegments = new List<LineSegment>();
  [HideInInspector]
  public bool isGrowing = true;
  [HideInInspector]
  public BranchComponent parentBranch;

  private LineRenderer lineRenderer;
  private float elapsedTime = 0f;
  private Vector3 nextTargetPosition;
  private int childBranchCount = 0;
  private bool hasSpawnedFoliage = false;

  public List<LineSegment> WorldLineSegments
  {
    get
    {
      return LineSegments.Select((ls) =>
      {
        return new LineSegment(
          transform.TransformPoint(ls.StartPoint),
          transform.TransformPoint(ls.EndPoint)
        );
      }).ToList();
    }
  }

  // Use this for initialization
  void Start()
  {
    lineRenderer = GetComponent<LineRenderer>();

    if (lineRenderer == null)
    {
      lineRenderer = this.gameObject.AddComponent<LineRenderer>();
      lineRenderer.textureMode = LineTextureMode.DistributePerSegment;
      lineRenderer.material = branchMaterial;
      lineRenderer.numCornerVertices = 25;
      lineRenderer.useWorldSpace = false;
    }

    Vector3[] positions = new Vector3[] {
      new Vector3(0, 0, 0)
    };

    lineRenderer.positionCount = positions.Length;
    lineRenderer.SetPositions(positions);

    // Add Ourselves to the Branch Manager
    BranchManager.instance.AddBranch(this);
  }

  // Update is called once per frame
  void Update()
  {
    if (isGrowing)
    {
      // Throttle updates to 60fps
      float throttledDeltaTime = Mathf.Clamp(Time.deltaTime, 0, 1f / 60f);
      Vector3 topPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
      Vector3[] linePositions;
      Vector3 newTopPosition;
      int linePositionsSize = lineRenderer.positionCount;

      if (lineRenderer.positionCount == 1 || topPosition.x == nextTargetPosition.x && topPosition.y == nextTargetPosition.y)
      {
        //Generate  new
        nextTargetPosition = generateNextPoint(topPosition);

        //Add a new position to our linePositions
        linePositionsSize += 1;
        //Duplicate Top Position
        newTopPosition = topPosition;
        LineSegments.Add(new LineSegment(topPosition, newTopPosition));

        elapsedTime = 0f;
      }
      else
      {
        // Lerp to Target!!
        elapsedTime += throttledDeltaTime;
        float positionOfGrowthTime = elapsedTime / secondsBetweenSegments;
        Vector3 prevPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
        newTopPosition = Vector3.Lerp(prevPosition, nextTargetPosition, positionOfGrowthTime);

        //Update our last Line Segment
        LineSegment currentSegment = LineSegments.Last();
        LineSegments.Last().EndPoint = newTopPosition;


      }

      //Once we hit our length, stop growing
      if (newTopPosition.y >= maxHeight)
      {
        isGrowing = false;
      }

      //Reset Positions
      linePositions = new Vector3[linePositionsSize];
      lineRenderer.GetPositions(linePositions);
      linePositions[linePositions.Length - 1] = newTopPosition;
      lineRenderer.positionCount = linePositions.Length;
      float branchHeight = Vector3.Distance(newTopPosition, linePositions[0]);
      float baseWidth = Mathf.Clamp((branchHeight / maxHeight) * maxBaseWidth, tipWidth, maxBaseWidth);
      lineRenderer.widthCurve = new AnimationCurve(
        new Keyframe(0f, baseWidth),
        new Keyframe(1f, tipWidth)
      );
      lineRenderer.SetPositions(linePositions);


      // Rules:
      //  1. Only branches below MAX_BRANCH_DEPTH can Generate Branches
      //  2. For The Trunk (branchLevel == 0) we need to wait until we have enough Base Segments
      //  3. For child branches, we spawn branches between the set Segments Between Branches, and
      //      for segments on the trunk after the first, we need to make sure to account for the Base Trunk
      //      segments.
      if (branchLevel <= MAX_BRANCH_DEPTH &&
         (branchLevel == 0 && childBranchCount == 0 && LineSegments.Count == TRUNK_BASE_SEGMENTS) ||
         ((childBranchCount * SEGMENTS_BETWEEN_BRANCH_POINTS) + (branchLevel == 0 ? TRUNK_BASE_SEGMENTS : SEGMENTS_BETWEEN_BRANCH_POINTS) == LineSegments.Count))
      {

        GameObject selectionPoint = Instantiate(selectionPointPrefab, LineSegments.Last().StartPoint, Quaternion.identity);
        selectionPoint.transform.parent = transform;
        selectionPoint.transform.localPosition = LineSegments.Last().StartPoint;
        selectionPoint.GetComponent<SelectionPointInteractions>().parentBranch = this;

        childBranchCount += 1;
      }
    }

    if (!isGrowing && !hasSpawnedFoliage && BranchManager.instance.IsGameOver())
    {
      spawnFoliage();
    }
  }

  public void AddBranch(Vector3 worldStartPoint, Vector3 worldDirection)
  {
    GameObject newChild = new GameObject();
    newChild.transform.parent = transform;

    Vector3 localPoint = transform.InverseTransformPoint(worldStartPoint);
    Vector3 localDirection = transform.InverseTransformDirection(worldDirection);
    localDirection.z = 0.0f;
    float branchRotation = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
    // Our branch is rendered straight "up" locally, so the target
    //  direction needs to have 90 degrees pulled off to account
    //  for it.
    branchRotation -= 90f;
    // If we are in a child level, we need to add the parent branch
    //  rotation object to make sure we are calculated in nested
    //  rotations due to calculating from world units.
    branchRotation += transform.rotation.eulerAngles.z;

    newChild.transform.rotation = Quaternion.Euler(0f, 0f, branchRotation);
    newChild.transform.localPosition = localPoint;

    float remainingBranchDistanceRatio = (maxHeight - newChild.transform.localPosition.y) / maxHeight;
    BranchComponent newBranch = newChild.AddComponent<BranchComponent>();
    newBranch.branchMaterial = new Material(branchMaterial);
    newBranch.branchLevel = branchLevel + 1;
    newBranch.maxBaseWidth = maxBaseWidth * CHILD_BRANCH_SCALE_FACTOR * remainingBranchDistanceRatio;
    newBranch.maxHeight = maxHeight * CHILD_BRANCH_SCALE_FACTOR;
    newBranch.maxLeftRightDistance = maxLeftRightDistance * CHILD_BRANCH_SCALE_FACTOR;
    newBranch.tipWidth = tipWidth * CHILD_BRANCH_SCALE_FACTOR;
    newBranch.selectionPointPrefab = selectionPointPrefab;
    newBranch.parentBranch = this;
    newBranch.animatedFlowerPrefab = animatedFlowerPrefab;
    newBranch.animatedLeafPrefab = animatedLeafPrefab;
  }

  private void spawnFoliage()
  {
    LineSegment end = null, before = null;
    end = LineSegments.Last();
    if(end == null){
      return;
    }

    if (LineSegments.Count() >= 2)
    {
      before = LineSegments.Last((ls) => ls != end);
    }

    if (branchLevel == 0)
    {
      if (before != null)
      {
        spawnLeavesOverSegment(before, 2f, 0.05f);
      }
      if (end != null)
      {
        spawnLeavesOverSegment(end, 2f, 0.5f);
        spawnFlowersOverSegment(end, 1.5f, 1f);
      }
    }
    else if(branchLevel == 1){
      int i = 0;
      foreach(LineSegment ls in LineSegments){
        if(i != 0 && ls != end){
          spawnLeavesOverSegment(ls, 2f, 0.05f * (i+1));
        }
        i++;
      }
      spawnLeavesOverSegment(end, 2f, 0.25f);
    }
    else if (branchLevel > 1)
    {
      spawnFlowersOverSegment(end, 4f * branchLevel, 0.5f * branchLevel);
    }



    hasSpawnedFoliage = true;
  }

  private string[] leafAniNames = new string[]{
    "leaf-1",
    "leaf-2",
    "leaf-3",
    "leaf-4",
    "leaf-5"
  };

  private string[] flowerAniNames = new string[] {
    "Bloom 1",
    "Bloom 2",
    "Bloom 3"
  };

  private void spawnFlowersOverSegment(LineSegment ls, float density, float baseDelay)
  {
    spawnPrefabOverSegment(animatedFlowerPrefab, flowerAniNames, ls, density, 1.75f, baseDelay);
  }
  private void spawnLeavesOverSegment(LineSegment ls, float density, float baseDelay)
  {
    spawnPrefabOverSegment(animatedLeafPrefab, leafAniNames, ls, density, 0.05f, baseDelay);
  }

  private void spawnPrefabOverSegment(GameObject prefab, string[] animationNames, LineSegment ls, float density, float bufferBetween, float baseDelay)
  {
    Vector3 dir = ls.Direction().normalized;
    float length = ls.Length();

    float stepMagnitude = length / density;
    float placementOffset = stepMagnitude * 0.25f;
    dir *= stepMagnitude;
    Vector3 stepVector = dir;

    List<Vector3> points = new List<Vector3>();
    points.Add(ls.StartPoint);
    while (dir.magnitude <= stepMagnitude)
    {
      points.Add(ls.StartPoint + dir);
      dir += stepVector;
    }

    foreach (Vector3 point in points)
    {
      int numSpawns = Random.Range(1, Mathf.CeilToInt(1.25f * density));
      int added = 0;
      while (added < numSpawns)
      {

        float xOff = Random.Range(-bufferBetween, bufferBetween);
        float yOff = Random.Range(-bufferBetween, bufferBetween);
        Vector3 targetLocalPosition = point + Vector3.ClampMagnitude(new Vector3(xOff, yOff, 0f), bufferBetween);
        
        GameObject leaf = Instantiate(prefab, targetLocalPosition, Quaternion.identity, transform);
        leaf.transform.localPosition = targetLocalPosition;
        DelayedAnimationComponent delay = leaf.AddComponent<DelayedAnimationComponent>();
        delay.delaySeconds = Random.Range(0.25f, 2f) + baseDelay;
        delay.animationName = animationNames[Random.Range(0, animationNames.Length)];
        added++;
      }
    }
  }

  private bool isVectorEmpty(Vector3 inVector)
  {
    return inVector.x == 0f && inVector.y == 0f && inVector.z == 0f;
  }
  private Vector3 generateNextPoint(Vector3 lastPoint)
  {
    if (lineRenderer.positionCount == 1)
    {
      return new Vector3(firstPosition.x, firstPosition.y, 0f);
    }

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
