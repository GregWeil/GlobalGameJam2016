using UnityEngine;
using System.Collections;

public class GodConstruction : MonoBehaviour {

    public GameObject block;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, -0.5f));
            float distance;
            if (plane.Raycast(ray, out distance)) {
                Collider2D col = Physics2D.OverlapPoint(ray.GetPoint(distance), LayerMask.GetMask("Solid"));
                if (col == null) {
                    GameObject obj = (GameObject)Instantiate(block);
                    Vector3 pos = ray.GetPoint(distance);
                    obj.transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0.0f);
                    obj.name = block.name;
                } else {
                    Debug.Log(col);
                }
            }
        }
	}
}
