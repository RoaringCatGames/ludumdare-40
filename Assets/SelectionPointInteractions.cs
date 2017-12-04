using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionPointInteractions : MonoBehaviour
{
	public BranchComponent parentBranch;
	public Material directionArrowMaterial;
	public float secondsTillDeath = 5f;

  private bool isUserDragging = false;
	private bool isUrgent = false;

  private Vector3 directionFromSelectionPoint;
	private LineRenderer directionRenderer;
	private Animator animator;

	private float elapsedTime = 0f;

	void Start(){
		animator = GetComponent<Animator>();
	}

  // Update is called once per frame
  void Update()
  {
		//Maximum time of 30fps
		elapsedTime += TimeUtils.ThrottledDelta(1f/30f);

		if(!isUrgent && elapsedTime >= ((2f/3f)*secondsTillDeath)){
			animator.Play("selection-urgent-pulse");
		}

    if (isUserDragging)
    {
      if (Input.GetMouseButtonDown(1))
      {
        //Right CLicking cancels Dragging
        isUserDragging = false;
				directionFromSelectionPoint = transform.position;
				Destroy(directionRenderer);
				directionRenderer = null;				
      }
      else if (Input.GetMouseButtonUp(0))
      {
        isUserDragging = false;
        var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directionFromSelectionPoint = position - transform.position;
				Destroy(directionRenderer);
				directionRenderer = null;
				
        if(parentBranch != null){
					parentBranch.AddBranch(transform.position, directionFromSelectionPoint);
					Destroy(gameObject);
				}
      }else{
				if(directionRenderer != null){
					var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					var distanceVector = position - transform.position;
					distanceVector.z = 0.0f; //clear z Axis just in case
					var clampedDistance = distanceVector.normalized * (parentBranch.maxHeight * BranchComponent.CHILD_BRANCH_SCALE_FACTOR);
					var targetEndpoint = transform.position + clampedDistance;
					directionRenderer.SetPositions(new Vector3[]{
						transform.position,
						new Vector3(targetEndpoint.x, targetEndpoint.y, transform.position.z)
					});
					directionRenderer.positionCount = 2;
				}
			}
    } else if(elapsedTime >= secondsTillDeath) {
			Destroy(gameObject);
		}

  }

  public void OnMouseDown()
  {
    isUserDragging = true;
		directionRenderer = gameObject.AddComponent<LineRenderer>();
		Renderer renderer = GetComponent<Renderer>();
		directionRenderer.sortingOrder = renderer.sortingOrder + 1;
		directionRenderer.sortingLayerName = renderer.sortingLayerName;
		//directionRenderer.sortingOrder = -20;
		// directionRenderer.sortingLayerName = "Trunk";
		directionRenderer.material = directionArrowMaterial;
		directionRenderer.startWidth = 0.1f;
		directionRenderer.endWidth = 0.1f;
  }

}
