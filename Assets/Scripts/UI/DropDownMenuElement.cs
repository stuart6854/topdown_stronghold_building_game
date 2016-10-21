using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropDownMenuElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public bool IsMouseOver = false;

	public void OnPointerEnter(PointerEventData eventData) {
		IsMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		IsMouseOver = false;
	}

}
