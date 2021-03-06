using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

	[SerializeField]
	private Text Title_Text;
	[SerializeField]
	public Text Body_Text;

	private float xShift = 40;
	private float yShift = -80;

	private CanvasGroup canvasGroup;

	void Start() {
		canvasGroup = GetComponent<CanvasGroup>();
	}

	private void LateUpdate() {
		UpdateTooltipPosition();
		UpdateTooltip();
	}

	private void UpdateTooltipPosition() {
		Vector3 pos = Input.mousePosition + new Vector3(xShift, yShift);

		Vector3[] corners = new Vector3[4];
		((RectTransform) transform).GetWorldCorners(corners);
		float width = corners[2].x - corners[0].x;
		float height = corners[1].y - corners[0].y;

		float distPastX = pos.x + width - Screen.width;
		if(distPastX > 0)
			pos = new Vector3(pos.x - distPastX, pos.y, pos.z);

		float distPastY = pos.y - height;
		if(distPastY < 0)
			pos = new Vector3(pos.x, pos.y - distPastY, pos.z);

		this.transform.position = pos;
	}

	private void UpdateTooltip() {
		if(EventSystem.current.IsPointerOverGameObject()) {
			SetVisible(false); //Mouse is over UI
			return;
		}

		Vector2 mousePos = Input.mousePosition;
		if((mousePos.x < 0 || mousePos.x > Screen.width) || (mousePos.y < 0 || mousePos.y > Screen.height)) {
			SetVisible(false);
			return;
		}

		Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

		RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 1000);
		if(hit.collider == null) {
			SetVisible(false);
			return;
		}

		ObjectDataReference worldObjectRef = hit.transform.GetComponent<ObjectDataReference>();
		if(worldObjectRef == null) {
			SetVisible(false);
			return;//This object cant be linked to its data for some reason
		}
		
		if(worldObjectRef.WorldObject == null) {
			SetVisible(false);
			return; //The selection is attached to a null worldobject for some reason
		}

		ITooltip tooltipObj = worldObjectRef.WorldObject as ITooltip;
		if(tooltipObj == null) {
			SetVisible(false);
			return; //Worldobject doesnt inherit from ITooltip
		}

		SetTitle(tooltipObj.Tooltip_GetTitle());
		SetBody(tooltipObj.Tooltip_GetBodyText());
		SetVisible(true);
	}

	public void SetVisible(bool visible) {
		if(!visible)
			this.canvasGroup.alpha = 0;
		else
			this.canvasGroup.alpha = 1;
	}

	public void SetTitle(string title) {
		this.Title_Text.text = title;
	}

	public void SetBody(string body) {
		this.Body_Text.text = body;
	}

	public string GetTitle() {
		return this.Title_Text.text;
	}

	public string GetBody() {
		return this.Body_Text.text;
	}

}
