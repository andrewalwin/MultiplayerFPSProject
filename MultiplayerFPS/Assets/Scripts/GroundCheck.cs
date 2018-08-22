using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

    private PlayerMove playerMove;

	// Use this for initialization
	void Start () {
        playerMove = GetComponentInParent<PlayerMove>();
	}

    private void OnTriggerEnter(Collider other)
    {
        playerMove.Grounded();
    }
}
