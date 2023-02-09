using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour {

    [SerializeField] PlatesCounter platesCounter;
    [SerializeField] Transform counterTopPoint;
    [SerializeField] Transform plateVisualPrefab;

    private List<GameObject> plateVisualGameObjectList;
    private void Awake() {
        plateVisualGameObjectList = new List<GameObject>();
    }
    private void Start() {
        platesCounter.OnPlatesSpawned += PlatesCounter_OnPlatesSpawned;
        platesCounter.OnPlatesRemoved += PlatesCounter_OnPlatesRemoved;
    }

    private void PlatesCounter_OnPlatesRemoved(object sender, System.EventArgs e) {
        GameObject plateVisualGameObject = plateVisualGameObjectList[plateVisualGameObjectList.Count - 1];
        plateVisualGameObjectList.Remove(plateVisualGameObject);
        Destroy(plateVisualGameObject);
    }

    private void PlatesCounter_OnPlatesSpawned(object sender, System.EventArgs e) {
        Transform plateVisualTransform = Instantiate(plateVisualPrefab, counterTopPoint);

        float plateOffsetY = .1f;
        plateVisualPrefab.localPosition = new Vector3(0, plateVisualGameObjectList.Count * plateOffsetY, 0);
        plateVisualGameObjectList.Add(plateVisualTransform.gameObject);
    }
}
