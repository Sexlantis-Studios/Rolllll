using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTile : MonoBehaviour {

    Vector3 startPos;

    // Start is called before the first frame update
    void Start() {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if(transform.position != startPos) {
            transform.position = Vector3.MoveTowards(transform.position, startPos, 4f * Time.deltaTime);
        }
    }
}
