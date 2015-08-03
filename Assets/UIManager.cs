using UnityEngine;
using System.Collections;
using SimpleJSON;
public class UIManager : MonoBehaviour {
	public GameObject button;
	public GameObject canvas;


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
					GameObject clone = Instantiate(button);
					
					//Update the rect transform as per data
					var rt = clone.GetComponent<RectTransform>();
					rt.anchoredPosition = new Vector2(location["x"].AsFloat, location["y"].AsFloat);
					rt.sizeDelta = new Vector2(size["width"].AsFloat, size["height"].AsFloat);
					
					//Set the UI's parent to the canvas	
					clone.transform.SetParent(this.transform.parent);
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
