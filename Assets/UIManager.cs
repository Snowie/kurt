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

	//Children of ui containers use relative positions
	void panelLogic(RectTransform parent,JSONArray components) {
		foreach (JSONNode uiObj in components) {
			JSONNode location = uiObj ["location"];
			JSONNode size = uiObj ["size"];

			//Determine the type of ui object to render
			switch (uiObj ["type"]) {
				case "KTButton":
					UnityEngine.UI.Button buttonClone = Instantiate (button);
					buttonClone.transform.SetParent (parent);

					UnityEngine.UI.Text buttonText = buttonClone.transform.GetComponentInChildren<UnityEngine.UI.Text>();
					buttonText.text = uiObj["id"].ToString();

					//Update the rect transform as per data
					var buttonRT = buttonClone.GetComponent<RectTransform> ();
					buttonRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					buttonRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);
				break;

				case "KTInputField":
					UnityEngine.UI.InputField inputFieldClone = Instantiate (inputField);
					inputFieldClone.transform.SetParent (parent);

					//Update the rect transform as per data
					var inputFieldRT = inputFieldClone.GetComponent<RectTransform> ();
					inputFieldRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					inputFieldRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);
				break;

				case "KTLabel":
					UnityEngine.UI.Text textFieldClone = Instantiate (textField);
					textFieldClone.transform.SetParent (parent);

					textFieldClone.text = uiObj ["id"].ToString ();

					//Update the rect transform as per data
					var textFieldRT = textFieldClone.GetComponent<RectTransform> ();
					textFieldRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					textFieldRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);
				break;

				//Scrollable UI containers
				case "KTScrollView":
					GameObject scrollViewClone = Instantiate(scrollView);
					scrollViewClone.transform.SetParent(parent);

					//Update rect transform of scroll view
					var scrollViewRT = scrollViewClone.GetComponent<RectTransform>();
					scrollViewRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					scrollViewRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);

					//Update rect transform of panel
					RectTransform scrollPanel = scrollViewClone.transform.GetChild(0).GetComponent<RectTransform>();
					scrollPanel.sizeDelta += new Vector2(0, uiObj["panelsize"]["height"].AsFloat - size["height"].AsFloat);
					scrollPanel.localPosition = new Vector2(0, -1 * (uiObj["panelsize"]["height"].AsFloat - size["height"].AsFloat));

					panelLogic(scrollViewClone.transform.GetChild(0).GetComponent<RectTransform>(), (JSONArray)uiObj["children"]);
				break;

				//Static UI containers
				case "KTPanel":
					GameObject panelClone = Instantiate(panel);
					panelClone.transform.SetParent(parent);

					var panelRT = panelClone.GetComponent<RectTransform>();
					panelRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					panelRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);

					panelLogic(panelClone.GetComponent<RectTransform>(), (JSONArray)uiObj["children"]);
				break;

				//External widget loader, will not respect parent container dimensions yet!
				case "KTWidget":
					GameObject widgetPanel = Instantiate(panel);
					widgetPanel.transform.SetParent(parent);
					
					RectTransform widgetRT = widgetPanel.GetComponent<RectTransform>();	
					widgetRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					widgetRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);
					
					string widgetURL = uiObj["external"].ToString();
					WWW widgetResponse = new WWW(widgetURL);
					
					while(!widgetResponse.isDone)
						;
					
					var widgetJSON = JSON.Parse(widgetResponse.text);
					
					panelLogic(widgetPanel.GetComponent<RectTransform>(), widgetJSON);
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

		//Base container is the canvas
		panelLogic(this.transform.parent.GetComponent<RectTransform>(), ui);
	}

	// Update is called once per frame
	void Update () {

	}
}
