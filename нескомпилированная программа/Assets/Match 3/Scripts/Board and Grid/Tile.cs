using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
	private static Color selectedColor = new Color(.8f, .8f, .8f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;
	private bool matchFound = false;

	private Vector2[] nearDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() 
	{
		render = GetComponent<SpriteRenderer>();
    }

	private void Select() 
	{
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
	}

	private void Deselect() 
	{
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

	void OnMouseDown()
	{
		if (render.sprite == null || BoardManager.instance.IsOk)
		{
			return;
		}

		if (isSelected)
		{ 
			Deselect();
		}
		else
		{
			if (previousSelected == null)
			{ 
				Select();
			}
			else
			{
				if (GetNearTiles().Contains(previousSelected.gameObject))
				{
					SwapSprite(previousSelected.render);
					previousSelected.ClearMatches();
					ClearMatches();
					previousSelected.Deselect();
					Manager.instance.MoveCounter--;
				}
				else
				{
					previousSelected.GetComponent<Tile>().Deselect();
				}
			}
		}
	}

	public void SwapSprite(SpriteRenderer _render)
	{
		if (render.sprite == _render.sprite)
		{
			return;
		}

		Sprite tempSprite = _render.sprite;
		_render.sprite = render.sprite;
		render.sprite = tempSprite;
	}

	private GameObject GetNear(Vector2 Near)
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Near);
		if (hit.collider != null)
		{
			return hit.collider.gameObject;
		}
		return null;
	}

	private List<GameObject> GetNearTiles()
	{
		List<GameObject> neartTiles = new List<GameObject>();
		for (int i = 0; i < nearDirections.Length; i++)
		{
			neartTiles.Add(GetNear(nearDirections[i]));
		}
		return neartTiles;
	}

	private List<GameObject> FindMatch(Vector2 cast)
	{
		List<GameObject> matchingTiles = new List<GameObject>(); 
		RaycastHit2D hit = Physics2D.Raycast(transform.position, cast);
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
		{
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, cast);
		}
		return matchingTiles; 
	}

	private void ClearMatch(Vector2[] paths) 
	{
		List<GameObject> matchingTiles = new List<GameObject>();
		for (int i = 0; i < paths.Length; i++)
		{
			matchingTiles.AddRange(FindMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2)
		{
			for (int i = 0; i < matchingTiles.Count; i++)
			{
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
			}
			matchFound = true;
		}
	}

	public void ClearMatches()
	{
		if (render.sprite == null)
		{
			return;
		}
		
		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		
		if (matchFound)
		{
			render.sprite = null;
			matchFound = false;
		}
	}
}