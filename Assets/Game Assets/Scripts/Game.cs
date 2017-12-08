using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class Game : MonoBehaviour {

	public GameObject nametag;
	public GameObject mainCamera;

	private GameObject terrain;
	private TerrainData tData;

	WebSocket w;
	bool isReady = false;
	Dictionary<string, PlayerData> activePlayers = new Dictionary<string, PlayerData>();

	IEnumerator Start() {
		w = new WebSocket(new Uri(Config.WSAddress));
		yield return StartCoroutine(w.Connect());

		w.SendString(Config.loginInfo);

		while(true) {
			string msg = w.RecvString();
			if(msg != null) {
				Debug.Log("Received: " + msg);
				string reply = wsCallback(msg);

				if(reply != null)
					w.SendString(reply);
			}
			if (w.error != null) {
				Debug.LogError("Error: "+w.error);
				break;
			}
			yield return 0;
		}
	}

	public string wsCallback(string data) {
		string reply = null;
		if(data.StartsWith("{")) {
			Triple<string, bool, string> decoded = WS.decode(data);
			if(decoded.A == "login") {
				if(decoded.B) {
					isReady = true;
				} else {
					SceneManager.LoadScene("LoginScreen");
				}
			} else if(decoded.A == "uinfo") {
				Debug.Log("Received player info: " + decoded.C);
				JSONObject j = new JSONObject(decoded.C);
				string pguid = j.GetField("guid").str;
				PlayerModels.ModelAssoc[] pmodels = GetComponent<PlayerModels>().models;
				Destroy(activePlayers[pguid].obj);

				// Set player model
				foreach(PlayerModels.ModelAssoc ma in pmodels) {
					if(ma.name.ToLower() == j.GetField("model").str.ToLower()) {
						activePlayers[pguid].obj = Instantiate(ma.model);
					}
				}

				// Attach nametag
				GameObject pNametag = Instantiate(nametag);
				pNametag.GetComponent<TextMesh>().text = j.GetField("name").str;
				pNametag.GetComponent<CopyRotation>().target = mainCamera;
				pNametag.GetComponent<UnityStandardAssets.Utility.FollowTarget>().target = activePlayers[pguid].obj.transform;

				// Attach movement controller
				CharacterMovementController cmc = activePlayers[pguid].obj.AddComponent<CharacterMovementController>();

			}
		} else {
			if(data.StartsWith("m")) {
				// Movement
				string guid = data.Substring(1, 36);
				string[] coords = data.Substring(37).Split(' ');

				if(activePlayers.ContainsKey(guid)) {
					// Player exists, update coords
					activePlayers[guid].coords.x = float.Parse(coords[0]);
					activePlayers[guid].coords.y = float.Parse(coords[2]); // Flip Y and Z
					activePlayers[guid].coords.z = float.Parse(coords[1]); // I like Z-up, but Unity is Y-up
					activePlayers[guid].rot = Quaternion.Euler(0, float.Parse(coords[3]), 0);
				} else {
					// Player doesn't exist, create a new GameObject
					PlayerData p = new PlayerData();
					p.guid = guid;
					p.name = "Player";
					Vector3 loc = new Vector3(float.Parse(coords[0]), float.Parse(coords[2]), float.Parse(coords[1])); // Also same thing here, flip Y and Z
					p.coords = loc;
					p.rot = Quaternion.Euler(0, float.Parse(coords[3]), 0);
					p.obj = Instantiate(GetComponent<PlayerModels>().models[0].model);

					activePlayers.Add(guid, p);
					reply = "{\"s\":\"uinfo\",\"d\":\"" + guid + "\"}";
				}
				if(activePlayers[guid].obj != null) {
					CharacterMovementController cmc = activePlayers[guid].obj.GetComponent<CharacterMovementController>();
					if(cmc == null) {
						activePlayers[guid].obj.transform.position = activePlayers[guid].coords;
						activePlayers[guid].obj.transform.rotation = activePlayers[guid].rot;
					} else {
						cmc.moveTo(activePlayers[guid].coords, activePlayers[guid].rot);
					}
				}
			}
		}

		return reply;
	}
}
