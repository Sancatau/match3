using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour 
{
	public static BoardManager instance;
	public List<Sprite> characters = new List<Sprite>();
	public GameObject tile;
	public int xSize, ySize;

	private GameObject[,] tiles;

	public bool IsOk;

	void Start () 
	{
		instance = GetComponent<BoardManager>();

		Vector2 set = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(set.x, set.y);
    }
	void Update()
	{
		FindNullTiles();
	}

	private void CreateBoard (float Xset, float Yset) 
	{
		tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
		float startY = transform.position.y;

		Sprite[] previousLeft = new Sprite[ySize];
		Sprite previousDown = null;

		for (int x = 0; x < xSize; x++) 
		{
			for (int y = 0; y < ySize; y++) 
			{
				GameObject newTile = Instantiate(tile, new Vector3(startX + (Xset * x), startY + (Yset * y), 0), tile.transform.rotation); //prefab
				tiles[x, y] = newTile;

				newTile.transform.parent = transform;

				List<Sprite> possibleCharacters = new List<Sprite>();
				possibleCharacters.AddRange(characters);

				possibleCharacters.Remove(previousLeft[y]);
				possibleCharacters.Remove(previousDown);
				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];

				newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
				previousLeft[y] = newSprite;
				previousDown = newSprite;
			}
        }
    }

	public void FindNullTiles()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
				{
					ShiftTilesDown(x, y);
					break;
				}
			}
		}
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				tiles[x, y].GetComponent<Tile>().ClearMatches();
			}
		}
	}

	private void ShiftTilesDown(int x, int yStart)
	{
		IsOk = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < ySize; y++)
		{
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null)
			{
				nullCount++;
			}
			renders.Add(render);
		}

		for (int i = 0; i < nullCount; i++)
		{
			Manager.instance.Score += 50;

			for (int k = 0; k < renders.Count - 1; k++)
			{
				renders[k].sprite = renders[k + 1].sprite;
				renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
			}
		}
		IsOk = false;
	}

	private Sprite GetNewSprite(int x, int y)
	{
		List<Sprite> possibleCharacters = new List<Sprite>();
		possibleCharacters.AddRange(characters);

		if (x > 0)
		{
			possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (x < xSize - 1)
		{
			possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (y > 0)
		{
			possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}

		return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
	}

}
