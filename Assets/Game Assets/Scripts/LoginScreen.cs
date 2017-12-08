using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class LoginScreen : MonoBehaviour {

	WebSocket w;
	string loginInfo = null;
	GameObject loginPanel = null;
	GameObject loadingPanel = null;

	IEnumerator Start() {
		loginPanel = GameObject.Find("LoginPanel");
		loadingPanel = GameObject.Find("LoadingOverlay");

		loadingPanel.SetActive(false);

		w = new WebSocket(new Uri(Config.WSAddress));
		yield return StartCoroutine(w.Connect());

		while (true) {
			string reply = w.RecvString();
			if(reply != null) {
				Debug.Log("Received: "+reply);
				if(reply.StartsWith("{")) {
					wsCallback(reply);
				}
			}
			if(loginInfo != null) {
				w.SendString(loginInfo);
				Config.loginInfo = loginInfo;
				loginInfo = null;
			}
			if (w.error != null) {
				Debug.LogError ("Error: "+w.error);
				break;
			}
			yield return 0;
		}
	}

	public void wsCallback(string data) {
		Text t = GameObject.Find("tInfo").GetComponent<Text>();
		Triple<string, bool, string> decoded = WS.decode(data);
		if(decoded.A == "login") {
			if(decoded.B) {
				t.text = "Login success!";
				t.color = Color.green;

				loadingPanel.SetActive(true);
				loginPanel.SetActive(false);

				SceneManager.LoadScene("Game");
			} else {
				t.text = "Login failed.";
				t.color = Color.red;

				loadingPanel.SetActive(false);
				loginPanel.SetActive(true);
			}
		}
	}

	public void doLogin() {
		Debug.Log("Logging in...");

		string uname = GameObject.Find("iUsername").GetComponent<InputField>().text;
		string passwd = GameObject.Find("iPassword").GetComponent<InputField>().text;

		JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		j.AddField("s", "login");
		JSONObject d = new JSONObject(JSONObject.Type.OBJECT);
		j.AddField("d", d);
		d.AddField("user", uname);
		d.AddField("pass", passwd);

		string jdat = j.Print().ToString();

		loginInfo = jdat;
	}
}
