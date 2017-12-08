using UnityEngine;
using System.Collections;

public class PlayerModels : MonoBehaviour {
	[System.Serializable]
	public struct ModelAssoc {
		public string name;
		public GameObject model;
	}

	public ModelAssoc[] models = new ModelAssoc[5];

	[System.Serializable]
	public struct AnimAssoc {
		public string name;
		public Animation model;
	}

	public AnimAssoc[] animations = new AnimAssoc[5];
}
