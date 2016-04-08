using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodConstruction : MonoBehaviour {

    public Transform selector;
    public GameObject block;

	public AudioSource pickSound, placeSound;
	[Range (0f, 1f)]
	public float amtScreenCoverage = 0.2f;

    GameObject selectedBlock = null;
    Vector3 selectorPosition = Vector3.zero;
    Vector3 selectorVelocity = Vector3.zero;

	GodHandAnimation anim = null;
	bool animHasBlock = false;
	Vector3 animReleasePos = Vector3.zero;

	int maxBlocks;
	List<GameObject> placedBlocks;

	// Use this for initialization
	void Start () {
		anim = GetComponent<GodHandAnimation> ();
		placedBlocks = new List<GameObject>();
		Vector2 gBounds = GameMaster.gm.getGameDimensions ();
		Debug.Log (gBounds);
		float blockSizeX = block.transform.localScale.x;
		float blockSizeY = block.transform.localScale.y;
		float numSquares = (gBounds.x / blockSizeX) * (gBounds.y / blockSizeY);
		maxBlocks = Mathf.RoundToInt (numSquares * amtScreenCoverage);
		Debug.Log (maxBlocks);
	}

	// Update is called once per frame
	void Update () {
		if (GameMaster.gm.paused) { return; }
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, new Vector3(0f, 0f, -0.5f));
        float distance;
        if (plane.Raycast(ray, out distance)) {
            Vector3 pos = ray.GetPoint(distance);
            pos = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), 0.0f);

            var col = Physics2D.OverlapPoint(ray.GetPoint(distance), LayerMask.GetMask("Solid", "Reserved"));
            if (selectedBlock != null) {
                //We're holding something, update it's position and drop
                if (col != null) {
                    //Revert to last safe position if something is here
                    pos = selectorPosition;
				}
				if (animHasBlock) {
					anim.SetGoal (animReleasePos);
				}
				if (Input.GetMouseButton (0)) {
					animReleasePos = pos;
				} else if (anim.IsReady() && animHasBlock) {
					//Check for players or the idol in the way
					var hit = Physics2D.BoxCast(animReleasePos, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0f, LayerMask.GetMask("Player", "Idol"));
					if (hit.collider == null) {
						//Release the block
						selectedBlock.transform.position = animReleasePos;
						selectedBlock.GetComponent<Collider2D> ().enabled = true;

						if (!placedBlocks.Contains (selectedBlock)) {
							placedBlocks.Add (selectedBlock);
//							if ((placedBlocks.Count % 15) == 0) {
//								Debug.Log (placedBlocks.Count);
//							}
						}
//						Debug.Log (selectedBlock.transform.position);

						selectedBlock = null;
						anim.SetExit ();
						animHasBlock = false;
						anim.Open ();
						placeSound.Play ();
					}
				}
			} else if (Input.GetMouseButtonDown (0) && anim.IsRetracted() && !animHasBlock) {
				//We aren't holding something, either spawn or grab
				if (col == null) {
					selectedBlock = (GameObject)Instantiate (block, pos, Quaternion.identity);
					selectedBlock.GetComponent<Collider2D> ().enabled = false;
					selectedBlock.name = block.name;
					anim.SetGoal (pos);
					animHasBlock = true;
					anim.Close ();
				} else if (col.GetComponent<GodStaticItem> () == null) {
					selectedBlock = col.gameObject;
					selectedBlock.GetComponent<Collider2D> ().enabled = false;

					anim.SetGoal (pos);
					animHasBlock = false;
					anim.Open ();
				}
				animReleasePos = pos;
			} else if (anim.IsRetracted() && !animHasBlock) {
				anim.SetGoalX (pos.x);
            }

			// if more blocks have been placed than the max, remove the oldest block
			if (placedBlocks.Count > maxBlocks) {
				GameObject oldest = placedBlocks [0];
				placedBlocks.RemoveAt (0);
				Destroy (oldest);
//				Debug.Log ("destroyed block");
			}

            //Smoothly move the selector
            selector.position = Vector3.SmoothDamp(selector.position, pos, ref selectorVelocity, 0.05f);
            selectorPosition = pos;

			//Move block with hand
			if (selectedBlock != null) {
				if (animHasBlock) {
					selectedBlock.transform.position = anim.GetPosition ();
				} else if (anim.IsReady()) {
					animHasBlock = true;
					anim.Close ();
					pickSound.Play ();
				}
			}
        }
	}
}
