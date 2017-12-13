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
  public bool isAutomated = false;
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

  private LineRenderer _lineRenderer;
  private float _elapsedTime = 0f;
  private Vector3 _nextTargetPosition;
  private int _childBranchCount = 0;
  private bool _hasSpawnedFoliage = false;
  private string[] _leafAniNames = new string[]{
    "leaf-1",
    "leaf-2",
    "leaf-3",
    "leaf-4",
    "leaf-5"
  };

  private string[] _flowerAniNames = new string[] {
    "Bloom 1",
    "Bloom 2",
    "Bloom 3"
  };

  private LineSegment _lastSegment;
  private List<LineSegment> _worldLineSegments = new List<LineSegment>();

  [HideInInspector]
  public List<LineSegment> WorldLineSegments
  {
    get
    {
      return _worldLineSegments;
    }
  }

  // Use this for initialization
  void Start()
  {
    _lineRenderer = GetComponent<LineRenderer>();

    if (_lineRenderer == null)
    {
      _lineRenderer = this.gameObject.AddComponent<LineRenderer>();
      _lineRenderer.textureMode = LineTextureMode.DistributePerSegment;
      _lineRenderer.material = branchMaterial;
      _lineRenderer.numCornerVertices = 10;
      _lineRenderer.useWorldSpace = false;
    }

    Vector3[] positions = new Vector3[] {
      new Vector3(0, 0, 0)
    };

    _lineRenderer.positionCount = positions.Length;
    _lineRenderer.SetPositions(positions);

    if(!isAutomated){
      // Add Ourselves to the Branch Manager
      BranchManager.instance.AddBranch(this);
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (isGrowing)
    {
      // Throttle updates to 60fps
      float throttledDeltaTime = TimeUtils.ThrottledDelta(1f / 30f);
      Vector3 topPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);
      Vector3[] linePositions;
      Vector3 newTopPosition;
      int linePositionsSize = _lineRenderer.positionCount;

      if (_lineRenderer.positionCount == 1 || topPosition.x == _nextTargetPosition.x && topPosition.y == _nextTargetPosition.y)
      {
        //Generate  new
        _nextTargetPosition = _generateNextPoint(topPosition);

        //Add a new position to our linePositions
        linePositionsSize += 1;
        //Duplicate Top Position
        newTopPosition = topPosition;
        //LineSegments.Add(new LineSegment(topPosition, newTopPosition));
        _addLineSegment(topPosition, newTopPosition);

        _elapsedTime = 0f;
      }
      else
      {
        // Lerp to Target!!
        _elapsedTime += throttledDeltaTime;
        float positionOfGrowthTime = _elapsedTime / secondsBetweenSegments;
        Vector3 prevPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 2);
        newTopPosition = Vector3.Lerp(prevPosition, _nextTargetPosition, positionOfGrowthTime);

        //Update our last Line Segment
        _updateLatestLineSegment(topPosition);
      }

      //Once we hit our length, stop growing
      if (newTopPosition.y >= maxHeight)
      {
        isGrowing = false;
      }

      //Reset Positions
      linePositions = new Vector3[linePositionsSize];
      _lineRenderer.GetPositions(linePositions);
      linePositions[linePositions.Length - 1] = newTopPosition;
      _lineRenderer.positionCount = linePositions.Length;
      float branchHeight = Vector3.Distance(newTopPosition, linePositions[0]);
      float baseWidth = Mathf.Clamp((branchHeight / maxHeight) * maxBaseWidth, tipWidth, maxBaseWidth);
      _lineRenderer.widthCurve = new AnimationCurve(
        new Keyframe(0f, baseWidth),
        new Keyframe(1f, tipWidth)
      );
      _lineRenderer.SetPositions(linePositions);


      // Rules:
      //  1. Only branches below MAX_BRANCH_DEPTH can Generate Branches
      //  2. For The Trunk (branchLevel == 0) we need to wait until we have enough Base Segments
      //  3. For child branches, we spawn branches between the set Segments Between Branches, and
      //      for segments on the trunk after the first, we need to make sure to account for the Base Trunk
      //      segments.
      if (branchLevel <= MAX_BRANCH_DEPTH &&
         (branchLevel == 0 && _childBranchCount == 0 && LineSegments.Count == TRUNK_BASE_SEGMENTS) ||
         ((_childBranchCount * SEGMENTS_BETWEEN_BRANCH_POINTS) + (branchLevel == 0 ? TRUNK_BASE_SEGMENTS : SEGMENTS_BETWEEN_BRANCH_POINTS) == LineSegments.Count))
      {
        if(isAutomated){
          _addBranchLocal(_lastSegment.StartPoint, Random.Range(0f, 1f) > 0.5f ? 90f : -90f);
        }else{
          GameObject selectionPoint = Instantiate(selectionPointPrefab, _lastSegment.StartPoint, Quaternion.identity);
          selectionPoint.transform.parent = transform;
          selectionPoint.transform.localPosition = _lastSegment.StartPoint;
          selectionPoint.GetComponent<SelectionPointInteractions>().parentBranch = this;
        }
        _childBranchCount += 1;
      }
    }

    if((isAutomated && !isGrowing && !_hasSpawnedFoliage) ||
       (!isGrowing && !_hasSpawnedFoliage && BranchManager.instance != null && BranchManager.instance.IsGameOver()))
    {
      _spawnFoliage();
    }
  }

  public void AddBranch(Vector3 worldStartPoint, Vector3 worldDirection)
  {

    Vector3 localPoint = transform.InverseTransformPoint(worldStartPoint);
    Vector3 localDirection = transform.InverseTransformDirection(worldDirection);
    localDirection.z = 0.0f;
    float branchRotation = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
    // Our branch is rendered straight "up" locally, so the target
    //  direction needs to have 90 degrees pulled off to account
    //  for it.
    branchRotation -= 90f;

    _addBranchLocal(localPoint, branchRotation);
  }

  private void _addLineSegment(Vector3 start, Vector3 end){
    _lastSegment = new LineSegment(start, end);  
    LineSegments.Add(_lastSegment);

    // Calculate the World Segment
    _worldLineSegments.Add(new LineSegment(
      transform.TransformPoint(start),
      transform.TransformPoint(end)
    ));
  }

  private void _updateLatestLineSegment(Vector3 newEndpoint){
    _lastSegment.EndPoint = newEndpoint;
    _worldLineSegments.Last().EndPoint = transform.TransformPoint(newEndpoint);
  }

  private void _addBranchLocal(Vector3 localPoint, float rotation){

    GameObject newChild = new GameObject();
    newChild.transform.parent = transform;

    // If we are in a child level, we need to add the parent branch
    //  rotation object to make sure we are calculated in nested
    //  rotations due to calculating from world units.
    float branchRotation = rotation + transform.rotation.eulerAngles.z;

    newChild.transform.rotation = Quaternion.Euler(0f, 0f, branchRotation);
    newChild.transform.localPosition = localPoint;

    float remainingBranchDistanceRatio = (maxHeight - newChild.transform.localPosition.y) / maxHeight;
    BranchComponent newBranch = newChild.AddComponent<BranchComponent>();
    
    if(isAutomated){
      newBranch.isAutomated = isAutomated;
      newBranch.secondsBetweenSegments = secondsBetweenSegments;
    }
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

  private void _spawnFoliage()
  {
    float baseFlowerDensity = 6f;
    LineSegment end = null, before = null;
    end = _lastSegment;
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
        //spawnLeavesOverSegment(before, baseFlowerDensity, 0.05f);
      }
      if (end != null)
      {
        //spawnLeavesOverSegment(end, 2f, 0.5f);
        _spawnFlowersOverSegment(end, baseFlowerDensity, 1f);
      }
    }
    // else if(branchLevel == 1){
    //   // int i = 0;
    //   // foreach(LineSegment ls in LineSegments){
    //   //   if(i != 0 && ls != end){
    //   //     spawnLeavesOverSegment(ls, 2f, 0.05f * (i+1));
    //   //   }
    //   //   i++;
    //   // }
    //   // spawnLeavesOverSegment(end, 2f, 0.25f);
    // }
    else //if (branchLevel > 1)
    {
      _spawnFlowersOverSegment(end, baseFlowerDensity * branchLevel, 0.75f * branchLevel);
    }
    
    _hasSpawnedFoliage = true;
  }

  private void _spawnFlowersOverSegment(LineSegment ls, float density, float baseDelay)
  {
    _spawnPrefabOverSegment(animatedFlowerPrefab, _flowerAniNames, ls, density, 1.75f, baseDelay);
  }
  private void _spawnLeavesOverSegment(LineSegment ls, float density, float baseDelay)
  {
    _spawnPrefabOverSegment(animatedLeafPrefab, _leafAniNames, ls, density, 1.75f, baseDelay);
  }

  private void _spawnPrefabOverSegment(GameObject prefab, string[] animationNames, LineSegment ls, float density, float bufferBetween, float baseDelay)
  {
    Vector3 dir = ls.Direction().normalized;
    float length = ls.Length();

    float stepMagnitude = length / density;
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
        delay.delaySeconds = Random.Range(0f, 0.75f);// IGNORE BASE DELAY FOR NOW + baseDelay;
        delay.animationName = animationNames[Random.Range(0, animationNames.Length)];
        added++;
      }
    }
  }

  private bool _isVectorEmpty(Vector3 inVector)
  {
    return inVector.x == 0f && inVector.y == 0f && inVector.z == 0f;
  }
  private Vector3 _generateNextPoint(Vector3 lastPoint)
  {
    if (_lineRenderer.positionCount == 1)
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
