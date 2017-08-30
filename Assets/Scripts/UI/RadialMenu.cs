using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour {

	private List<MenuButton> MenuButtons = new List<MenuButton>();

	private Vector2 MousePosition;
	public Vector2 RelativeMousePos;
	public Vector2 Centre;
	private float Radius;

	public int ButtonCount;
	public int CurrMenuItem;
	public int OldMenuItem;

	public float angle;

	void Start() {
		CurrMenuItem = 0;
		OldMenuItem = 0;

		Centre = transform.position;
	}

	void Update() {
		GetCurrentMenuItem();
		if(Input.GetMouseButtonDown(0))//Left
			ButtonAction();
	}

	public void GetCurrentMenuItem() {
		if(MenuButtons.Count == 0)
			return;

		MousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

		RelativeMousePos = MousePosition - Centre;

		if(RelativeMousePos.magnitude < Radius / 2f || RelativeMousePos.magnitude > Radius) {
			MenuButton current = MenuButtons[OldMenuItem];
			current.Texture.color = current.NormalColor;
			CurrMenuItem = -1;
			return;
		}

		angle = (Mathf.Atan2(RelativeMousePos.y, RelativeMousePos.x) * Mathf.Rad2Deg) + 90;

		if(angle < 0)
			angle += 360;
		
		CurrMenuItem = (int) (angle / (360f / ButtonCount));

		if(CurrMenuItem >= ButtonCount)
			CurrMenuItem = 0;
		else if(CurrMenuItem < 0)
			CurrMenuItem = ButtonCount - 1;
		
		MenuButton old = MenuButtons[OldMenuItem];
		old.Texture.color = old.NormalColor;
		OldMenuItem = CurrMenuItem;

		MenuButton curr = MenuButtons[CurrMenuItem];
		curr.Texture.color = curr.HighlightedColor;
	}

	public void ButtonAction() {
		if(CurrMenuItem == -1) {//No MenuItem selected
			Destroy(this.gameObject); //Close Menu
			return;
		}

		MenuButton button = MenuButtons[CurrMenuItem];
		button.Texture.color = button.PressedColor;

		if(button.OnClick != null) {
			button.OnClick();
			Destroy(this.gameObject);
		}
	}

	public void SetCentre(float x, float y) {
		this.Centre = new Vector2(x, y);
	}

	public void SetCircleRadius(float radius) {
		this.Radius = radius;
	}

	public void AddButton(MenuButton button) {
		button.Texture.color = button.NormalColor;

		MenuButtons.Add(button);

		ButtonCount = MenuButtons.Count;
	}

	[Serializable]
	public class MenuButton {

		public string Name;
		public Image Texture;
		public Color NormalColor;
		public Color HighlightedColor;
		public Color PressedColor;
		public Action OnClick;

		public MenuButton(string name, Image texture, Color normalColor, Color highlightedColor, Color pressedColor, Action onClick) {

			this.Name = name;
			this.Texture = texture;
			this.NormalColor = normalColor;
			this.HighlightedColor = highlightedColor;
			this.PressedColor = pressedColor;
			this.OnClick = onClick;

		}

	}
	
}
