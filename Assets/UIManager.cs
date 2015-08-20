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
    public JSONNode theme = new JSONNode();

	//Children of ui containers use relative positions
	IEnumerator panelLogic(RectTransform parent, JSONArray components) {
		foreach (JSONNode uiObj in components) {
			JSONNode location = uiObj ["location"];
			JSONNode size = uiObj ["size"];

			//Determine the type of ui object to render
			switch (uiObj ["type"]) {
				case "KTButton":
					UnityEngine.UI.Button buttonClone = Instantiate (button);
					buttonClone.transform.SetParent (parent);

					UnityEngine.UI.Text buttonText = buttonClone.transform.GetComponentInChildren<UnityEngine.UI.Text>();
					buttonText.text = uiObj["id"];

					//Update the rect transform as per data
					var buttonRT = buttonClone.GetComponent<RectTransform> ();
					buttonRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					buttonRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);

                    buttonClone.image.color = ConvertColor(theme ["KTButton"], "color", buttonClone.image.color);
                    buttonText.color = ConvertColor(theme["KTButton"], "text", buttonText.color);
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

					textFieldClone.text = uiObj ["id"];

					//Update the rect transform as per data
					var textFieldRT = textFieldClone.GetComponent<RectTransform> ();
					textFieldRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					textFieldRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);
				break;

				//Scrollable UI containers
				case "KTScrollView":
					GameObject scrollViewClone = Instantiate(scrollView);
					scrollViewClone.transform.SetParent(parent);
					
					UnityEngine.UI.ScrollRect cloneRect = scrollViewClone.GetComponent<UnityEngine.UI.ScrollRect>();
					cloneRect.horizontal = uiObj["scrollable"]["horizontal"].AsBool;
					cloneRect.vertical = uiObj["scrollable"]["vertical"].AsBool;

					//Update rect transform of scroll view
					var scrollViewRT = scrollViewClone.GetComponent<RectTransform>();
					scrollViewRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					scrollViewRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);

					//Update rect transform of panel
					RectTransform scrollPanel = scrollViewClone.transform.GetChild(0).GetComponent<RectTransform>();
					scrollPanel.sizeDelta += new Vector2(0, uiObj["panelsize"]["height"].AsFloat - size["height"].AsFloat);
					scrollPanel.localPosition = new Vector2(0, -1 * (uiObj["panelsize"]["height"].AsFloat - size["height"].AsFloat));

					StartCoroutine(panelLogic(scrollViewClone.transform.GetChild(0).GetComponent<RectTransform>(), (JSONArray)uiObj["children"]));
				break;

				//Static UI containers
				case "KTPanel":
					GameObject panelClone = Instantiate(panel);
					panelClone.transform.SetParent(parent);

					var panelRT = panelClone.GetComponent<RectTransform>();
					panelRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					panelRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);

					StartCoroutine(panelLogic(panelClone.GetComponent<RectTransform>(), (JSONArray)uiObj["children"]));
				break;

				//External widget loader, will not respect parent container dimensions yet!
				case "KTWidget":
					GameObject widgetPanel = Instantiate(panel);
					widgetPanel.transform.SetParent(parent);
					
					RectTransform widgetRT = widgetPanel.GetComponent<RectTransform>();	
					widgetRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, location["x"].AsFloat, size["width"].AsFloat);
					widgetRT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, location["y"].AsFloat, size["height"].AsFloat);
					
					string widgetURL = uiObj["external"];
					
					//TODO: Change this to a post to pass arguments into destination JSON	
					WWW widgetResponse = new WWW(widgetURL);
					
					yield return widgetResponse;
					
					var widgetJSON = JSON.Parse(widgetResponse.text);
					
					if(widgetResponse.text == ""){
						Debug.Log("Empty Widget!");
						break;
					}
					
					JSONArray widgetArray = (JSONArray)widgetJSON ["widget"];
					
					StartCoroutine(panelLogic(widgetPanel.GetComponent<RectTransform>(), widgetArray));
				break;
			}
		}
	}

	IEnumerator changeUI(string uiPage) {
		//TODO: Clear ui container object.

		WWW uiResponse = new WWW (uiPage);
		
		//Wait to finish grabbing the UI
		yield return uiResponse;

		//Parse the response
		var uiJSON = JSON.Parse (uiResponse.text);

		//Get the array of ui elements
		JSONArray ui = (JSONArray)uiJSON ["ui"];
		
		//Base container is the canvas
		StartCoroutine(panelLogic(this.transform.parent.GetComponent<RectTransform>(), ui));
	}

    IEnumerator changeTheme(string uiTheme)
    {
        //TODO: Clear ui container object.

        WWW uiRes = new WWW(uiTheme);

        //Wait to finish grabbing the UI
        yield return uiRes;

        //Parse the response
        theme = JSON.Parse(uiRes.text);
    }

    Color ConvertColor(JSONNode node_theme, string colorName, Color color)
    {
        if(node_theme == null)
            return color;

        JSONNode node_color = node_theme[colorName];

        if (node_color == null)
            return color;

        JSONNode name = node_color ["name"];

        if (name != null)
        {
            switch ((string)name)
            {
                case "clear":
                    return Color.clear;
                    
                case "white":
                    return Color.white;
                  
                case "red":
                    return Color.red;
                    
                case "magenta":
                    return Color.magenta;
                    
                case "yellow":
                    return Color.yellow;
                    
                case "green":
                    return Color.green;
                    
                case "blue":
                    return Color.blue;
                    
                case "cyan":
                    return Color.cyan;
                    
                case "gray":
                case "grey":
                    return Color.gray;
                    
                case "black":
                    return Color.black;
            }
        }
        else
        {
            color.r = node_color["r"].AsFloat;
            color.g = node_color["g"].AsFloat;
            color.b = node_color["b"].AsFloat;
            color.a = node_color["a"].AsFloat;
        }

        return color;
    }

	// Use this for initialization
	void Start () {
        string currentUIPage = "http://kirsten398.github.io/kurt/index.html";
        string currentUITheme = "http://kirsten398.github.io/kurt/theme.html";
        StartCoroutine(changeTheme(currentUITheme));
		StartCoroutine(changeUI (currentUIPage));
	}

	// Update is called once per frame
	void Update () {

	}
}
