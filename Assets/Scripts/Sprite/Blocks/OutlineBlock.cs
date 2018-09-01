using UnityEngine;
using System.Collections;

public class OutlineBlock : BaseSprite {
	public Texture2D leftTop, left, leftBottom, bottom, rightBottom, right, rightTop, top;
	
	// Update is called once per frame
	void Update () {
		int x =(int) transform.position.x;
		int y = (int)transform.position.y;

		if (!Map.Instance.HasBlock (x - 1, y) && Map.Instance.HasBlock (x +1, y) && Map.Instance.HasBlock(x,y-1) && !Map.Instance.HasBlock(x,y+1) && leftTop!=null) {
			GetComponent<Renderer> ().material.mainTexture = leftTop;
		}
		else if (!Map.Instance.HasBlock (x - 1, y) && Map.Instance.HasBlock (x +1, y) && Map.Instance.HasBlock(x,y+1) && !Map.Instance.HasBlock(x,y-1) && leftBottom!=null) {
			GetComponent<Renderer> ().material.mainTexture = leftBottom;
		}
		else if(!Map.Instance.HasBlock (x - 1, y) && Map.Instance.HasBlock (x +1, y) && Map.Instance.HasBlock(x,y+1) && Map.Instance.HasBlock(x,y-1) && left!=null) {
			GetComponent<Renderer> ().material.mainTexture = left;
		}
		else if (Map.Instance.HasBlock (x - 1, y) && Map.Instance.HasBlock (x +1, y) && !Map.Instance.HasBlock(x,y-1) && Map.Instance.HasBlock(x,y+1) && bottom!=null) {
			GetComponent<Renderer> ().material.mainTexture = bottom;
		}
		else if (Map.Instance.HasBlock (x - 1, y) && !Map.Instance.HasBlock (x +1, y) && !Map.Instance.HasBlock(x,y-1) && Map.Instance.HasBlock(x,y+1) && rightBottom!=null) {
			GetComponent<Renderer> ().material.mainTexture = rightBottom;
		}
		else if (Map.Instance.HasBlock (x - 1, y) && !Map.Instance.HasBlock (x +1, y) && Map.Instance.HasBlock(x,y-1) && Map.Instance.HasBlock(x,y+1) && right!=null) {
			GetComponent<Renderer> ().material.mainTexture = right;
		}
		else if (Map.Instance.HasBlock (x - 1, y) && !Map.Instance.HasBlock (x +1, y) && Map.Instance.HasBlock(x,y-1) && !Map.Instance.HasBlock(x,y+1) && rightTop!=null) {
			GetComponent<Renderer> ().material.mainTexture = rightTop;
		}
		else if (Map.Instance.HasBlock (x - 1, y) && Map.Instance.HasBlock (x +1, y) && Map.Instance.HasBlock(x,y-1) && !Map.Instance.HasBlock(x,y+1) && top!=null) {
			GetComponent<Renderer> ().material.mainTexture = top;
		}

		if (!Map.Instance.HasBlock (x, y - 1) && !Map.Instance.HasBlock (x, y + 1)) {
			GetComponent<Renderer> ().material.mainTexture = top;
		}


		Destroy (this);
	}
}
