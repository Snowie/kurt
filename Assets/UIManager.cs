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

		while (!uiResponse.isDone)
			;

		var uiJSON = JSON.Parse (uiResponse.text);

		JSONArray tree = (JSONArray)uiJSON ["ui"];

		foreach(JSONNode obj in tree) {
			RectTransform uiTrans = new RectTransform();
			JSONNode location = obj["location"];
			JSONNode size = obj["size"];

			switch(obj["type"]) {
				case "KTButton" :
					GameObject clone = Instantiate(button);
					var rt = clone.GetComponent<RectTransform>();
					rt.anchoredPosition = new Vector2(location["x"].AsInt, location["y"].AsInt);
					rt.sizeDelta = new Vector2(size["width"].AsFloat, size["height"].AsFloat);
					clone.transform.SetParent(this.transform);
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
