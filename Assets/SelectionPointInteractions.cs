using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionPointInteractions : MonoBehaviour
{

  private bool isUserDragging = false;

  private Vector3 directionFromSelectionPoint;
	private LineRenderer directionRenderer;
  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

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
				
        //TODO: Trigger Branch Creation
      }else{
				if(directionRenderer != null){
					var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					directionRenderer.SetPositions(new Vector3[]{
						transform.position,
						new Vector3(mousePosition.x, mousePosition.y, 0f)
					});
					directionRenderer.positionCount = 2;
				}
			}


    }

  }

  public void OnMouseDown()
  {
    Logger.Log("SELECTION POINT STARTED!");
    isUserDragging = true;
		directionRenderer = gameObject.AddComponent<LineRenderer>();
		directionRenderer.startWidth = 0.1f;
		directionRenderer.endWidth = 0.1f;
  }

}
