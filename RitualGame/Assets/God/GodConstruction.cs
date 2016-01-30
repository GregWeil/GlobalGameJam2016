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
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, new Vector3(0f, 0f, -0.5f));
        float distance;
        if (plane.Raycast(ray, out distance)) {
            Vector3 pos = ray.GetPoint(distance);
            pos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0.0f);

            var col = Physics2D.OverlapPoint(ray.GetPoint(distance), LayerMask.GetMask("Solid"));
            if (selectedBlock != null) {
                //We're holding something, update it's position and drop
                if (col != null) {
                    //Revert to last safe position if something is here
                    pos = selectorPosition;
                }
                selectedBlock.transform.position = selector.position;
                if (!Input.GetMouseButton(0)) {
					//Check for players or the idol in the way
					var hit = Physics2D.BoxCast(pos, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f, LayerMask.GetMask("Player", "Idol"));
					if (hit.collider == null) {
						//Release the block
						selectedBlock.transform.position = pos;
						selectedBlock.GetComponent<Collider2D> ().enabled = true;
						selectedBlock = null;
					}
                }
            } else if (Input.GetMouseButton(0)) {
                //We aren't holding something, either spawn or grab
                if (col == null) {
                    var obj = (GameObject)Instantiate(block, pos, Quaternion.identity);
                    obj.name = block.name;
                } else {
					selectedBlock = col.gameObject;
					selectedBlock.GetComponent<Collider2D> ().enabled = false;
                }
            }

            //Smoothly move the selector
            selector.position = Vector3.SmoothDamp(selector.position, pos, ref selectorVelocity, 0.05f);
            selectorPosition = pos;
        }
	}
}
