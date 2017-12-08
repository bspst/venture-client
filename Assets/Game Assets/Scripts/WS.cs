using UnityEngine;
using System;
using System.Collections;

public class WS : MonoBehaviour {

	public static WebSocket w;
	//string sendQueue = null;

	// Use this for initialization
	public static void init(Action<string> callback) {
		Debug.Log("Starting WS Client");
		w = new WebSocket(new Uri(Config.WSAddress));
		//yield return StartCoroutine(w.Connect());
		w.Connect();

		while (true) {
			string reply = w.RecvString();
			if (reply != null) {
				Debug.Log("Received: " + reply);
				callback(reply);
			}
			if (w.error != null) {
				Debug.LogError("Error: " + w.error);
				break;
			}

			/*if(sendQueue != null) {
				Debug.Log("Sending message");
				w.SendString(sendQueue);
				sendQueue = null;
			}*/

			//yield return 0;
			return;
		}
		w.Close();
	}

	/*public bool send(string data) {
		if(sendQueue == null) sendQueue = data;
		else return false;
		return true;
	}*/

	public static Triple<string, bool, string> decode(string data) {
		JSONObject j = new JSONObject(data);
		string subj = j.GetField("s").str;
		bool status = j.GetField("t").b;
		string sdat = j.GetField("d").ToString();

		return new Triple<string, bool, string>(subj, status, sdat);
	}
}
