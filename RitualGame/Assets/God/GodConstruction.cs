using UnityEngine;
using System.Collections;

public class GodConstruction : MonoBehaviour {

    public Transform selector;
    public GameObject block;

    GameObject selectedBlock = null;
    Vector3 selectorPosition = Vector3.zero;
    Vector3 selectorVelocity = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, new Vector3(0f, 0f, -0.5f));
        float distance;
        if (plane.Raycast(ray, out distance)) {
            Vector3 pos = ray.GetPoint(distance);
            pos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0.0f);

            Collider2D col = Physics2D.OverlapPoint(ray.GetPoint(distance), LayerMask.GetMask("Solid"));
            if (selectedBlock != null) {
                if (col != null) {
                    pos = selectorPosition;
                }
                selectedBlock.transform.position = selector.position;
                if (!Input.GetMouseButton(0)) {
                    selectedBlock.transform.position = pos;
                    selectedBlock = null;
                }
            } else if (Input.GetMouseButton(0)) {
                if (col == null) {
                    GameObject obj = (GameObject)Instantiate(block, pos, Quaternion.identity);
                    obj.name = block.name;
                } else {
                    selectedBlock = col.gameObject;
                }
            }

                selector.position = Vector3.SmoothDamp(selector.position, pos, ref selectorVelocity, 0.05f);
            selectorPosition = pos;
        }
	}
}
