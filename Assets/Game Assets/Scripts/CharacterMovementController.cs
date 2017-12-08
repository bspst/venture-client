using UnityEngine;
using System.Collections;

public class CharacterMovementController : MonoBehaviour {
	public Transform target;
	public float smoothTime = 0.5F;
	private Vector3 velocity = Vector3.zero;

	private Vector3 targetLoc;
	private Quaternion targetRot;
	
	// Update is called once per frame
	void Update() {
		//Vector3 targetPos = target.TransformPoint(targetLoc);
		//transform.position = Vector3.SmoothDamp(transform.position, targetLoc, ref velocity, smoothTime);
		transform.position = Vector3.Lerp(transform.position, targetLoc, Time.deltaTime);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime);
	}

	public void moveTo(Vector3 loc, Quaternion rot) {
		//targetLoc = target.TransformPoint(loc);
		targetLoc = loc;
		targetRot = rot;
	}
}
