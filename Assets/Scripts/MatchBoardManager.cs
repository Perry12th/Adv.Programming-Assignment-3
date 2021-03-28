using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode { Easy, Medium, Hard};
public class MatchBoardManager : MonoBehaviour
{
    public static MatchBoardManager instance;
    [SerializeField]
    private List<Sprite> characters = new List<Sprite>();
    [SerializeField]
    public Sprite inMoveablecharacter;
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private int boardSizeX;
    [SerializeField]
    private int boardSizeY;
    private GameObject[,] tiles;
    [SerializeField]
    private Mode gameMode = Mode.Easy;
    public bool IsShifting { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        instance = GetComponent<MatchBoardManager>();
    }


    public void GameOver()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void StartGame()
    {
        Vector2 offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size * 1.2f;
        CreateBoard(offset.x, offset.y);
    }

    public Mode GameMode
    {
        get { return gameMode; }

        set
        {
            if (gameMode == value)
            {
                return;
            }

            else
            {
                gameMode = value;

                switch (gameMode)
                {
                    case (Mode.Easy):
                        UIManager.instance.StartingTime = 150;
                        UIManager.instance.RequiredScore = 4000;
                        Tile.matchRequirement = 2;
                        break;
                    case (Mode.Medium):
                        UIManager.instance.StartingTime = 120;
                        UIManager.instance.RequiredScore = 6000;
                        Tile.matchRequirement = 2;
                        break;
                    case (Mode.Hard):
                        UIManager.instance.StartingTime = 120;
                        UIManager.instance.RequiredScore = 7000;
                        Tile.matchRequirement = 3;
                        break;
                }
            }
        }
    }


    private void CreateBoard (float xOffset, float yOffset)
    {
        tiles = new GameObject[boardSizeX, boardSizeY];

        float startPointX = transform.position.x;
        float startPointY = transform.position.y;

        Sprite[] previousLeft = new Sprite[boardSizeY];
        Sprite previousBelow = null;

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(startPointX + (xOffset * x), startPointY + (yOffset * y), transform.position.z), tilePrefab.transform.rotation);
                tiles[x, y] = newTile;
                newTile.transform.parent = transform;

                List<Sprite> possibleCharacters = new List<Sprite>();
                possibleCharacters.AddRange(characters);
                if (gameMode != Mode.Easy)
                {
                    possibleCharacters.Add(inMoveablecharacter);
                }    

                possibleCharacters.Remove(previousLeft[y]);
                possibleCharacters.Remove(previousBelow);
                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (tiles[x,y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
    {
        IsShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < boardSizeY; y++)
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
            UIManager.instance.Score += 50;
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < renders.Count - 1; k++)
            {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, boardSizeY - 1);
            }
        }
        IsShifting = false;
    }

    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);
        if (gameMode != Mode.Easy)
        {
            possibleCharacters.Add(inMoveablecharacter);
        }

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < boardSizeX - 1)
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
