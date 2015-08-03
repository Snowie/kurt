using UnityEngine;
using System.Collections;
using SimpleJSON;
public class UIManager : MonoBehaviour {
	public UnityEngine.UI.Button button;
	public UnityEngine.UI.InputField inputField;
	public UnityEngine.UI.Text textField;


	// Use this for initialization
	void Start () {
		string url = "http://snowie.github.io/kurt/";
		WWW uiResponse = new WWW (url);

		//Wait to finish grabbing the UI
		while (!uiResponse.isDone)
			;

		//Parse the response
		var uiJSON = JSON.Parse (uiResponse.text);

		//Get the array of ui elements
		JSONArray ui = (JSONArray)uiJSON ["ui"];

		//Iterate over ui
		foreach(JSONNode uiObj in ui) {
			JSONNode location = uiObj["location"];
			JSONNode size = uiObj["size"];

			//Determine the type of ui object to render
			switch(uiObj["type"]) {
				case "KTButton" :
					UnityEngine.UI.Button buttonClone = Instantiate(button);
					
					//Update the rect transform as per data
					var buttonRT = buttonClone.GetComponent<RectTransform>();
					buttonRT.anchoredPosition = new Vector2(location["x"].AsFloat, location["y"].AsFloat);
					buttonRT.sizeDelta = new Vector2(size["width"].AsFloat, size["height"].AsFloat);
					
					//Set the UI's parent to the canvas	
					buttonClone.transform.SetParent(this.transform.parent);
				break;

				case "KTInputField":
					UnityEngine.UI.InputField inputFieldClone = Instantiate(inputField);
					//Update the rect transform as per data
					var inputFieldRT = inputFieldClone.GetComponent<RectTransform>();
					inputFieldRT.anchoredPosition = new Vector2(location["x"].AsFloat, location["y"].AsFloat);
					inputFieldRT.sizeDelta = new Vector2(size["width"].AsFloat, size["height"].AsFloat);
				
					//Set the UI's parent to the canvas	
					inputFieldClone.transform.SetParent(this.transform.parent);
				break;

				case "KTLabel":
					UnityEngine.UI.Text textFieldClone = Instantiate(textField);
					
					textFieldClone.text = uiObj["id"].ToString();

					//Update the rect transform as per data
					var textFieldRT = textFieldClone.GetComponent<RectTransform>();
					textFieldRT.anchoredPosition = new Vector2(location["x"].AsFloat, location["y"].AsFloat);
					textFieldRT.sizeDelta = new Vector2(size["width"].AsFloat, size["height"].AsFloat);
				
					//Set the UI's parent to the canvas	
					textFieldClone.transform.SetParent(this.transform.parent);
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
