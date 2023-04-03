using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {
    private Transform targetTransform;

    public void SetTargetTransform(Transform transform) {
        targetTransform = transform;
    }

    private void LateUpdate() {
        if (targetTransform != null) {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }
}
