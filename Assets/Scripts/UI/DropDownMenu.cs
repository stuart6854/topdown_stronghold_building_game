using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropDownMenu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public DropDownMenuElement dropDownMenu;

	private bool isMouseOver = false;

	public void OnPointerClick(PointerEventData eventData) {
		dropDownMenu.gameObject.SetActive(true);
	}

	private void Update() {
		if(!isMouseOver && !dropDownMenu.IsMouseOver)
			dropDownMenu.gameObject.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		isMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		isMouseOver = false;
	}

}
