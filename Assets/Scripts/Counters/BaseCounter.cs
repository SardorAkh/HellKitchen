using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BaseCounter : MonoBehaviour, IKitchenObjectParent {

    public static event EventHandler OnAnyObjectPlacedHere;

    public static void ResetStaticData() {
        OnAnyObjectPlacedHere = null;
    }

    [SerializeField]
    private Transform counterTopPoint;

    protected KitchenObject kitchenObject;
    public virtual void Interact(Player player) {

    }
    public virtual void InteractAlternate(Player player) {

    }
    public Transform GetKitchenObjectFollowTransform() {
        return counterTopPoint;
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }
    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
    }
    public void ClearKitchenObject() {
        this.kitchenObject = null;
    }
    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
