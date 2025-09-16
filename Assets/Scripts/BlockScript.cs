using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
	public List<GameObject> blocks = new ();
	public List<SpriteRenderer> spriteRenderers = new ();
	public List<BoxCollider2D> boxCollider2Ds = new ();
	public List<Animator> animators = new ();
	private bool rotated = false;
	public Sprite orangeSprite;
	public Sprite blueSprite;

	public Rigidbody2D rb2D;

	private Vector2[] originalPositions = new Vector2[4];
	public Vector2[] rotatedPositions = new Vector2[4];

	Color holdColor = new Color (1f, 1f, 1f, 0.25f);
	Color overlappingColor = new Color (1f, 0, 0, 0.25f);

	bool holding = false;
	[HideInInspector] public bool overlapping = false;


    void Start()
    {
		int i = 0;
		foreach (GameObject obj in blocks)
		{
			originalPositions[i] = obj.transform.localPosition;
			i++;
		}
    }

	void Update ()
	{
		foreach (GameObject obj in blocks)
		{
			// If parent scale is inverted, flip every child scale
			float xScale = transform.localScale.x * -1f;
			obj.transform.localScale = new Vector3 (xScale, 1, 1);
		}

		if (overlapping)
		{
			foreach (SpriteRenderer rend in spriteRenderers)
			{
				rend.color = overlappingColor;
			}
		}
		else if (holding)
		{
			foreach (SpriteRenderer rend in spriteRenderers)
			{
				rend.color = holdColor;
			}
		}
	}

    public void Rotate ()
    {
		rotated = !rotated;

		int i = 0;
        foreach (GameObject obj in blocks)
		{
			if (rotated)
				obj.transform.localPosition = rotatedPositions[i];
			else
				obj.transform.localPosition = originalPositions[i];
			i++;
		}
    }

	public void StartHold (int playerIndex)
	{
		holding = true;
		rb2D.bodyType = RigidbodyType2D.Kinematic;

		foreach (SpriteRenderer rend in spriteRenderers)
		{
			rend.sprite = playerIndex == 1 ? blueSprite : orangeSprite;
			rend.color = holdColor;
		}
		foreach (BoxCollider2D col in boxCollider2Ds)
		{
			col.isTrigger = true;
		}
		Debug.Log ("Start HOLD");
	}
	public void StopHold ()
	{
		foreach (BoxCollider2D col in boxCollider2Ds)
		{
			col.isTrigger = false;
		}
		holding = false;
		rb2D.bodyType = RigidbodyType2D.Dynamic;
		Debug.Log ("End HOLD");
	}

	public void AnimateAppear ()
	{
		Color alphaZero =  new Color (1f, 1f, 1f, 0f);
		foreach (SpriteRenderer rend in spriteRenderers)
		{
			rend.color = alphaZero;
		}
		StartCoroutine (AnimateAppearRoutine (0.1f));
	}

	private void OnDisable()
	{
		foreach (Animator anim in animators)
		{
			anim.enabled = false;
		}
	}

	IEnumerator AnimateAppearRoutine (float waitTime = 0.1f)
	{
		foreach (Animator anim in animators)
		{
			anim.enabled = true;
			anim.SetTrigger ("appear");
			Debug.Log ("APPEAR");
			yield return new WaitForSeconds (waitTime);
		}
	}
}
