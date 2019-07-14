using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator {
    public static GameObject CreateInObject (GameObject parent, GameObject child) {
        GameObject cell = GameObject.Instantiate (child) as GameObject;
        cell.transform.SetParent (parent.transform, false);
        PositionInitilazer pInit = new PositionInitilazer ();
        //pInit.FixedCenterKeepSize(cell.GetComponent<RectTransform>());
        return cell;
    }

    public static void DestroyAllChild (GameObject parent) {
        for (int i = 0; i < parent.transform.childCount; ++i)
            GameObject.Destroy (parent.transform.GetChild (i).gameObject);
    }

    public static void InactiveAllChild (GameObject parent, bool active) {
        for (int i = 0; i < parent.transform.childCount; ++i)
            parent.transform.GetChild (i).gameObject.SetActive (active);
    }

    public static void InactiveChild (GameObject[] parents, bool active) {
        for (int i = 0; i < parents.Length; ++i) {
            if (parents[i] != null)
                parents[i].gameObject.SetActive (active);
        }
    }

    public static GameObject Clone (GameObject go) {
        var clone = GameObject.Instantiate (go) as GameObject;
        clone.transform.parent = go.transform.parent;
        clone.transform.position = go.transform.position;
        clone.transform.rotation = go.transform.rotation;
        clone.transform.localScale = go.transform.localScale;
        return clone;
    }

    public static List<GameObject> GetAll (GameObject obj) {
        List<GameObject> allChildren = new List<GameObject> ();
        GetChildren (obj, ref allChildren);
        return allChildren;
    }

    public static void GetChildren (GameObject obj, ref List<GameObject> allChildren) {
        Transform children = obj.GetComponentInChildren<Transform> ();
        //子要素がいなければ終了
        if (children.childCount == 0) {
            return;
        }
        foreach (Transform ob in children) {
            allChildren.Add (ob.gameObject);
            GetChildren (ob.gameObject, ref allChildren);
        }
    }
}

public class PositionInitilazer {
    public void FixedCenter (RectTransform rect) {
        rect.anchoredPosition3D = Vector3.zero;
        rect.sizeDelta = Vector3.zero;
        rect.localScale = Vector3.one;
    }

    public void FixedCenterKeepSize (RectTransform rect) {
        rect.anchoredPosition3D = Vector3.zero;
        rect.localScale = Vector3.one;
    }
}