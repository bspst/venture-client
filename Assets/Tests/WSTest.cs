using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class WSTest : MonoBehaviour {
	WebSocket w;

	// Use this for initialization
	IEnumerator Start () {
		Debug.Log("Starting WS Client");
		w = new WebSocket(new Uri("ws://localhost:5000"));
		yield return StartCoroutine(w.Connect());
		//w.SendString("Hi there");
		//int i=0;
		while (true)
		{
			string reply = w.RecvString();
			if (reply != null)
			{
				Debug.Log("Received: "+reply);
				if(reply.StartsWith("{")) {
					Debug.Log("Attempting to decode JSON");
					JSONObject j = new JSONObject(reply);
					string subj = j.GetField("s").str;
					bool status = j.GetField("t").b;
					Debug.Log("Subj: " + subj);
					if(subj == "connection" && status) {
						Debug.Log("Connection successful!");
					} else if(subj == "login") {
						if(status) {
							Debug.Log("Login successful");
							GameObject.Find("tGUID").GetComponent<Text>().text = "GUID: " + j.GetField("d").str;
						} else {
							Debug.Log("Login failed");
						}
						Debug.Log(j.GetField("d").str);
					}
				} else {
					if(reply.StartsWith("m")) {
						
					}
				}
			}
			if (w.error != null)
			{
				Debug.LogError ("Error: "+w.error);
				break;
			}
			yield return 0;
		}
		w.Close();
	}

	public void login() {
		string uname = GameObject.Find("iUnameTxt").GetComponent<Text>().text;
		string passwd = GameObject.Find("iPassword").GetComponent<InputField>().text;

		JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
		j.AddField("s", "login");
		JSONObject d = new JSONObject(JSONObject.Type.OBJECT);
		j.AddField("d", d);
		d.AddField("user", uname);
		d.AddField("pass", passwd);

		string jdat = j.Print().ToString();
		w.SendString(jdat);

		Debug.Log("Logging in");
		Debug.Log(jdat);
	}
}
