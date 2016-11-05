using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuGenerator : MonoBehaviour {

	public Sprite RadialMenuItemTexture;

	public Color NormalColor;
	public Color HighlightedColor;
	public Color PressedColor;

	public float Size;

	void Start() {
//		Font font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

//		MenuItems.Add(new RadialMenuItem("Use", font, null));
//		MenuItems.Add(new RadialMenuItem("Repair", font, null));
//		MenuItems.Add(new RadialMenuItem("Dismantle", font, null));
//		MenuItems.Add(new RadialMenuItem("Cancel", font, null));
//		MenuItems.Add(new RadialMenuItem("Cancel", font, null));
//		MenuItems.Add(new RadialMenuItem("Cancel", font, null));

//		GenerateMenu(0.5f, 0.5f, 800, this.transform);
	}

	public GameObject GenerateMenu(RadialMenuItem[] MenuItems, float size, Transform parent) {
		this.Size = size;

		int buttons = MenuItems.Length;

		float sectorAngleDegrees = 360f / buttons;
		float CentreX = parent.transform.position.x;
		float CentreY = parent.transform.position.y;

		GameObject RadialMenu_GO = new GameObject("RadialMenu");
		RadialMenu_GO.transform.SetParent(parent, true);

		RectTransform radialMenu_RT = RadialMenu_GO.AddComponent<RectTransform>();
		radialMenu_RT.position = new Vector3(CentreX, CentreY, 0);

		RadialMenu radialMenu = RadialMenu_GO.AddComponent<RadialMenu>();
		radialMenu.SetCentre(CentreX, CentreY);
		radialMenu.SetCircleRadius(size / 2f);

		int i = 0;
		foreach(RadialMenuItem menuItem in MenuItems) {
			float angleDegrees = (i * sectorAngleDegrees);

			GameObject button = new GameObject(menuItem.Name);
			RectTransform rectTransform = button.AddComponent<RectTransform>();
			rectTransform.SetParent(RadialMenu_GO.transform, false);
			rectTransform.position = new Vector3(CentreX, CentreY, 0);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.sizeDelta = new Vector2(size, size);

			GameObject Text_GO = new GameObject("Text");
			RectTransform text_RT = Text_GO.AddComponent<RectTransform>();
			text_RT.SetParent(button.transform, false);
			//			text_RT.localPosition = new Vector3(size / 2f, size / 2f, 0);
			Text buttonText = Text_GO.AddComponent<Text>();
			buttonText.alignment = TextAnchor.MiddleCenter;
			buttonText.fontSize = 24;
			buttonText.horizontalOverflow = HorizontalWrapMode.Overflow;
			buttonText.text = menuItem.Name;
			buttonText.font = menuItem.Font;

			rectTransform.eulerAngles = new Vector3(0, 0, angleDegrees - 1);
			//			rectTransform.eulerAngles = new Vector3(0, 0, angle + (sectorAngle / 2f) - 1);

			Image buttonImage = button.AddComponent<Image>();
			buttonImage.sprite = RadialMenuItemTexture;
			buttonImage.type = Image.Type.Filled;
			buttonImage.fillOrigin = 2;
			buttonImage.fillAmount = (sectorAngleDegrees - 2) / 360f;

			float textX = CentreX + Mathf.Cos((angleDegrees + (90 - (sectorAngleDegrees / 2f))) * Mathf.Deg2Rad) * size / 2.75f;
			float textY = CentreY + Mathf.Sin((angleDegrees + (90 - (sectorAngleDegrees / 2f))) * Mathf.Deg2Rad) * size / 2.75f;

			text_RT.eulerAngles = Vector3.zero;
			text_RT.position = new Vector3(textX, textY, 0);
			//			Debug.Log(menuItem.Name + ": " + angleDegrees + " | " + textX + ", " + textY);

			radialMenu.AddButton(new RadialMenu.MenuButton(menuItem.Name, buttonImage, NormalColor, HighlightedColor, PressedColor, null));

			i++;
		}

		return RadialMenu_GO;
	}

//	private void OnDrawGizmosSelected() {
//		Gizmos.color = Color.magenta;
//
//		float sectorAngleDegrees = 360f / MenuItems.Count;
//		float CentreX = transform.position.x;
//		float CentreY = transform.position.y;
//
//		int i = 0;
//		foreach(RadialMenuItem menuItem in MenuItems) {
//			float angleDegrees = (i * sectorAngleDegrees + (90 - (sectorAngleDegrees / 2f)));
//
//			float x = CentreX + Mathf.Cos(angleDegrees * Mathf.Deg2Rad) * (Size / 4);
//			float y = CentreY + Mathf.Sin(angleDegrees * Mathf.Deg2Rad) * (Size / 4);
//			//			Debug.Log(i + " " + menuItem.Name + ": " + x + ", " + y + " | " + (angleDegrees));
//
//			Gizmos.DrawCube(new Vector3(x, y, 0), new Vector3(10f, 10f, 10f));
//			Handles.Label(new Vector3(x, y, 0), menuItem.Name);
//
//			i++;
//		}
//
//		Gizmos.DrawCube(new Vector3(CentreX, CentreY, 0), new Vector3(15f, 15f, 15f));
//	}

	public class RadialMenuItem {

		public string Name;
		public Font Font;
		public Action OnClick;

		public RadialMenuItem(string name, Font font, Action onClick) {
			this.Name = name;
			this.Font = font;
			this.OnClick = onClick;
		}

	}

}
