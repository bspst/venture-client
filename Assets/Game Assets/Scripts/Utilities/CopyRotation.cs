using UnityEngine;
using System.Collections;

public class CopyRotation : MonoBehaviour {

	public GameObject target;

	void Update() {
		gameObject.transform.rotation = target.transform.rotation;
	}
}
