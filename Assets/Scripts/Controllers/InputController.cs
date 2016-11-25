using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour {

	private const float CAMERA_SPEED = 1.5f;

	public LayerMask SelectionLayerMask;

	private Camera cam;

	//Data
	private bool isDragging = false;
	private Vector2 DragStart;

	private bool IsPanning = false;
	private Vector3 MousePosLastFrame;

	void Start () {
		cam = Camera.main;
	}
	
	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape)) {
			if(BuildController.Instance.GetBuildMode() != BuildMode.None) {
				BuildController.Instance.SetBuildMode(BuildMode.None);
			} else {
				//Show Escape Menu - Resume, Save, Load, Settings, Exit, etc.
			}
		}
		
		if(ConsoleController.Instance.IsVisble) {
			if(BuildController.Instance.GetBuildMode() != BuildMode.None) {
				BuildController.Instance.SetBuildMode(BuildMode.None);
			}
			return;
		}

		HandleDrag();
		HandleCameraZoom();
		HandleCameraMovement();

		HandleClicks();
	}

	private void HandleDrag() {
	    if(BuildController.Instance.GetBuildMode() == BuildMode.None)
	        return;

		if(EventSystem.current.IsPointerOverGameObject())
			return;

	    if(BuildController.Instance.GetBuildMethod() == BuildMethod.Single || BuildController.Instance.GetBuildMode() == BuildMode.Character) {
	        if(Input.GetMouseButtonUp(0)) {
	            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
	            BuildController.Instance.DoAction(pos, Vector2.zero);
	        }
	    } else {
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

		if(Input.GetMouseButton(2)) {
			if(!IsPanning)
				MousePosLastFrame = Input.mousePosition;

			movement = MousePosLastFrame - new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

			cam.transform.Translate(movement * Time.deltaTime * CAMERA_SPEED * (cam.orthographicSize * .1f));

//			dX = MousePosLastFrame.x - Input.mousePosition.x;
//			dY = MousePosLastFrame.y - Input.mousePosition.y;
//
//			movement = new Vector3(dX * CAMERA_SPEED * Time.deltaTime, dY * CAMERA_SPEED * Time.deltaTime, 0);
//			cam.transform.position += movement * cam.orthographicSize;
//
			MousePosLastFrame = Input.mousePosition;

			IsPanning = true;
		} else {
			IsPanning = false;
		}
	}

	private void HandleClicks() {
		if(BuildController.Instance.GetBuildMode() != BuildMode.None)
			return; //We are in build mode, so no selection can happen

		if(EventSystem.current.IsPointerOverGameObject())
			return; //Mouse is over a UI Element

		if(!Input.GetMouseButtonUp(0) && !Input.GetMouseButtonUp(1))
			return; //Neither Left or Right Mouse Buttons were clicked

		//Get World Coords from click position
		Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);

		//Get the clicked WorldObject
		WorldObject selection = null;
		RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 10000, SelectionLayerMask);
		if(hit.collider != null) {
			//We hit something
			ObjectDataReference worldObjectRef = hit.transform.GetComponent<ObjectDataReference>();
			if(worldObjectRef == null)
				return; //This object cant be linked to its data for some reason

			//Get selection tile
			Tile tile = WorldController.Instance.GetTileAt(worldObjectRef.X, worldObjectRef.Y);
			if(tile == null)
				return; //The selection is attached to a null tile for some reason

//			switch(worldObjectRef.ObjectType) {
//				case WorldObjectType.InstalledObject:
//					selection = tile.GetInstalledObject();
//					break;
//				case WorldObjectType.LooseItem:
//					selection = tile.GetLooseItem();
//					break;
//				case WorldObjectType.Character:
//					//TODO: Need a way to retrieve character
//					break;
//			}
		}

		if(selection == null)
			return; //Nothing was selected

		//Logic based of whether it was a left or right click
		if(Input.GetMouseButtonUp(0)) {
			//Left


		}else if(Input.GetMouseButtonUp(1)) {
			//Right

			IContextMenu contextMenuObj = (IContextMenu) selection;
			if(contextMenuObj == null)
				return;

			UIController.Instance.GenerateRadialMenu(contextMenuObj.MenuOptions_ContextMenu());
		}
	}

}
