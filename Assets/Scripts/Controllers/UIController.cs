using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RadialMenuGenerator))]
public class UIController : MonoBehaviour {

	public static UIController Instance;

	public Canvas UICanvas;
	public Transform RadialMenuParent;

	public GameObject BuildButtonPrefab;
	public Transform TilesDropdown;
	public Transform InstalledObjectDropdown;

	private RadialMenuGenerator RadialMenuGen;

	private GameObject CurrentRadialMenu;
	private Transform RadialMenuTracker;

	private void Awake() {
		Instance = this;

		RadialMenuGen = GetComponent<RadialMenuGenerator>();
	}
	
	void Start () {
		CreateBuildButtons();
	}

	void Update() {
		if(CurrentRadialMenu != null && RadialMenuTracker != null) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint(RadialMenuTracker.position);
			CurrentRadialMenu.transform.position = screenPos;
			SetRadialMenuCentre(screenPos);
		}
	}

	private void CreateBuildButtons() {
		foreach(KeyValuePair<string, Definition> pair in Defs.Definitions) {
			Definition def = pair.Value;
			string type = def.Properties.DefCategory;
			if(type != "Tile" && type != "InstalledObject")
				continue;

			if(!def.Properties.ContainsXMLTag("Constructable"))
				continue;

			GameObject btn_obj = Instantiate(BuildButtonPrefab);
			btn_obj.name = def.Properties.DefName + "_btn";

			Image image = btn_obj.transform.GetChild(0).GetComponent<Image>();
			if(image != null) {
				Sprite sprite = SpriteController.GetSprite(def.Properties.DefName + "_icon");
				if(sprite != null) {
					image.sprite = sprite;
				}
			}

			Button button = btn_obj.GetComponent<Button>();
			if(type == "Tile") {
				button.onClick.AddListener(delegate () { BuildController.Instance.PlaceTile(def.Properties.DefName); });
				btn_obj.transform.SetParent(TilesDropdown, false);
			} else {
				button.onClick.AddListener(delegate () { BuildController.Instance.PlaceInstalledObject(def.Properties.DefName); });
				btn_obj.transform.SetParent(InstalledObjectDropdown, false);
			}
		}
	}

	public void GenerateRadialMenu(Vector2 screenPos, RadialMenuItem[] menuItems) {
		DestroyRadialMenu();

		CurrentRadialMenu = RadialMenuGen.GenerateMenu(menuItems, Screen.height / 2f, RadialMenuParent);
		CurrentRadialMenu.transform.position = screenPos;
	}

	public void DestroyRadialMenu() {
		if(CurrentRadialMenu != null)
			Destroy(CurrentRadialMenu);
		this.CurrentRadialMenu = null;
		this.RadialMenuTracker = null;
	}

	public void SetRadialMenuTracker(Transform tracker) {
		this.RadialMenuTracker = tracker;
	}

	public void SetRadialMenuCentre(Vector2 pos) {
		if(CurrentRadialMenu == null)
			return;

		CurrentRadialMenu.GetComponent<RadialMenu>().SetCentre(pos.x, pos.y);
	}

	public GameObject GetCurrentRadialMenu() {
		return CurrentRadialMenu;
	}

	public Transform GetRadialMenuTracker() {
		return RadialMenuTracker;
	}

}
