using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour {

    private ConsoleController Console;

    public GameObject ViewContainer;
    public Text ConsoleLogText;
    public InputField InputField;

    void Awake() {
        Console = new ConsoleController();
        if(Console != null) {
            Console.logChanged += onLogChanged;
        }

        UpdateLogString(Console.Log);
    }

    ~DevConsole() {
        Console.logChanged -= onLogChanged;
    }

    void Update() {
        if(Input.GetKeyUp("`")) {
            InputField.text = "";
            ToggleVisibility();
        }

        if(Input.GetKeyUp(KeyCode.Escape) && ViewContainer.activeSelf == true) {
            //Unfocus Inputfield
            EventSystem.current.SetSelectedGameObject(null, null);
        }
    }

    void ToggleVisibility() {
        SetVisibility(!ViewContainer.activeSelf);
    }

    void SetVisibility(bool visible) {
        ViewContainer.SetActive(visible);
	    Console.IsVisble = visible;
    }

    void onLogChanged(string[] newLog) {
        UpdateLogString(newLog);
    }

    void UpdateLogString(string[] newLog) {
        if(newLog == null) {
            ConsoleLogText.text = "";
        } else {
            ConsoleLogText.text = string.Join("\n", newLog);
        }
    }

    public void EnterCommand() {
        if(string.IsNullOrEmpty(InputField.text)) {
            return;
        }

        Console.RunCommandString(InputField.text);
        InputField.text = "";
        //Re-Focus Inputfield
        EventSystem.current.SetSelectedGameObject(InputField.gameObject, null);
        InputField.OnPointerClick(new PointerEventData(EventSystem.current));
    }

}
