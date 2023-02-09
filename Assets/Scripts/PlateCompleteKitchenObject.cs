using System;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteKitchenObject : MonoBehaviour {
    [Serializable]
    public struct KitchenObjectSO_GameObject {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectList;
    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (KitchenObjectSO_GameObject item in kitchenObjectSO_GameObjectList) {
            item.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedd_EventArgs e) {
        foreach (KitchenObjectSO_GameObject item in kitchenObjectSO_GameObjectList) {
            if (item.kitchenObjectSO == e.kitchenObjectSO) {
                item.gameObject.SetActive(true);
            }
        }
    }
}
