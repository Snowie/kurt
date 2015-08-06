using UnityEngine;
using System.Collections;
using SimpleJSON;

//TODO: Keep UI elements in a container for easy destruction in case of page change
public class UIManager : MonoBehaviour {
	//Prefab vars set in editor for instantiation
	public UnityEngine.UI.Button button;
	public UnityEngine.UI.InputField inputField;
	public UnityEngine.UI.Text textField;
	public GameObject scrollView;
	public GameObject panel;
	public RectTransform canvasTransform;

	//Children of ui containers use relative positions
	void panelLogic(RectTransform parent,JSONArray components) {
		foreach (JSONNode uiObj in components) {
			JSONNode location = uiObj ["location"];
			JSONNode size = uiObj ["size"];
			Vector2 posRelatizer = new Vector2(parent.position.x - parent.sizeDelta.x/2, parent.position.y + parent.sizeDelta.y/2);

			//Determine the type of ui object to render
			switch (uiObj ["type"]) {
				case "KTButton":
					UnityEngine.UI.Button buttonClone = Instantiate (button);

					UnityEngine.UI.Text buttonText = buttonClone.transform.GetComponentInChildren<UnityEngine.UI.Text>();
					buttonText.text = uiObj["id"].ToString();

					//Update the rect transform as per data
					var buttonRT = buttonClone.GetComponent<RectTransform> ();
					buttonRT.anchoredPosition = new Vector2(location["x"].AsFloat, -1 * location["y"].AsFloat) + posRelatizer;
					buttonRT.sizeDelta = new Vector2 (size ["width"].AsFloat, size ["height"].AsFloat);

					//Set the UI's parent to the canvas
					buttonClone.transform.SetParent (parent);
				break;

				case "KTInputField":
					UnityEngine.UI.InputField inputFieldClone = Instantiate (inputField);

					//Update the rect transform as per data
					var inputFieldRT = inputFieldClone.GetComponent<RectTransform> ();
					inputFieldRT.anchoredPosition = new Vector2(location["x"].AsFloat, -1 * location["y"].AsFloat) + posRelatizer;
					inputFieldRT.sizeDelta = new Vector2 (size ["width"].AsFloat, size ["height"].AsFloat);

					//Set the UI's parent to the canvas
				inputFieldClone.transform.SetParent (parent);
				break;

				case "KTLabel":
					UnityEngine.UI.Text textFieldClone = Instantiate (textField);

					textFieldClone.text = uiObj ["id"].ToString ();

					//Update the rect transform as per data
					var textFieldRT = textFieldClone.GetComponent<RectTransform> ();
					textFieldRT.anchoredPosition = new Vector2(location["x"].AsFloat, -1 * location["y"].AsFloat) + posRelatizer;
					textFieldRT.sizeDelta = new Vector2 (size ["width"].AsFloat, size ["height"].AsFloat);

					//Set the UI's parent to the canvas
				textFieldClone.transform.SetParent (parent);
				break;

				//Scrollable UI containers
				case "KTScrollView":
					GameObject scrollViewClone = Instantiate(scrollView);

					//Update rect transform of scroll view
					var scrollViewRT = scrollViewClone.GetComponent<RectTransform>();
					scrollViewRT.anchoredPosition = new Vector2(location["x"].AsFloat, -1 * location["y"].AsFloat) + posRelatizer;
					scrollViewRT.sizeDelta = new Vector2(size["width"].AsFloat, size["height"].AsFloat);

					scrollViewClone.transform.SetParent(parent);

					//Update rect transform of panel
					RectTransform scrollPanel = scrollViewClone.transform.GetChild(0).GetComponent<RectTransform>();
					scrollPanel.sizeDelta = new Vector2(uiObj["panelsize"]["width"].AsFloat, uiObj["panelsize"]["height"].AsFloat);
					scrollPanel.anchoredPosition = new Vector2(0, -1 *scrollPanel.sizeDelta.y/2);

					panelLogic(scrollViewClone.transform.GetChild(0).GetComponent<RectTransform>(), (JSONArray)uiObj["children"]);
				break;

				//Static UI containers
				case "KTPanel":
					GameObject panelClone = Instantiate(panel);

					var panelRT = panelClone.GetComponent<RectTransform>();
					panelRT.anchoredPosition = new Vector2(location["x"].AsFloat, -1 * location["y"].AsFloat) + posRelatizer;
					panelRT.sizeDelta = new Vector2(size["width"].AsFloat, size["height"].AsFloat);
					panelClone.transform.SetParent(parent);

					panelLogic(panelClone.GetComponent<RectTransform>(), (JSONArray)uiObj["children"]);
				break;
			}
		}
	}

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

		panelLogic(canvasTransform, ui);
	}

	// Update is called once per frame
	void Update () {

	}
}
