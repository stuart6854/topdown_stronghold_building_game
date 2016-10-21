using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour {

	private const float CAMERA_SPEED = 1.5f;

	private Camera cam;

	//Data
	private bool isDragging = false;
	private Vector2 DragStart;

	void Start () {
		cam = Camera.main;
	}
	
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape)) {
			if(BuildController.Instance.IsInBuildMode()) {
				BuildController.Instance.SetInBuildMode(false);
			} else {
				//Show Escape Menu - Resume, Save, Load, Settings, Exit, etc.
			}
		}

		HandleDrag();
		HandleCameraZoom();
		HandleCameraMovement();
	}

	private void HandleDrag() {
		if(EventSystem.current.IsPointerOverGameObject())
			return;

		if(Input.GetMouseButtonDown(0)) {
			//Drag has started
			isDragging = true;
			DragStart = cam.ScreenToWorldPoint(Input.mousePosition);
		}

		if(isDragging) {
			BuildController.Instance.OnDragging(DragStart, cam.ScreenToWorldPoint(Input.mousePosition));
		}

		if(Input.GetMouseButtonUp(0) && isDragging) {
			//Drag has ended
			isDragging = false;
			BuildController.Instance.DoAction(DragStart, cam.ScreenToWorldPoint(Input.mousePosition));
		}
	}

	private void HandleCameraZoom() {
		float zoom = Input.GetAxisRaw("Zoom");

		cam.orthographicSize -= cam.orthographicSize * zoom;
		cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 3, 30);
	}

	private void HandleCameraMovement() {
		float dX = Input.GetAxisRaw("Horizontal");
		float dY = Input.GetAxisRaw("Vertical");

		Vector3 movement = new Vector3(dX * CAMERA_SPEED * Time.deltaTime, dY * CAMERA_SPEED * Time.deltaTime, 0);
		cam.transform.position += movement * cam.orthographicSize;
	}

}
