using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static Tile lastSelectedTile = null;
    private static Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    public static int matchRequirement = 2;

    private SpriteRenderer render;
    private bool isTitleSelected = false;
    private bool matchFound = false;

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }
    private void SelectTile()
    {
        isTitleSelected = true;
        render.color = selectedColor;
        lastSelectedTile = this;
        SFXManager.instance.PlaySFX(Clip.Select);
    }

    private void DeselectTile()
    {
        isTitleSelected = false;
        render.color = Color.white;
        lastSelectedTile = null;
    }

    void OnMouseDown()
    {
        if (render.sprite == null || MatchBoardManager.instance.IsShifting || UIManager.instance.gameOver || render.sprite == MatchBoardManager.instance.inMoveablecharacter)
        {
            return;
        }

        if (isTitleSelected)
        {
            DeselectTile();
        }

        else
        {
            if (lastSelectedTile == null)
            {
                SelectTile();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(lastSelectedTile.gameObject))
                {
                    Debug.Log("Switch sprite");
                    SpawnSprite(lastSelectedTile.render);
                    lastSelectedTile.ClearAllMatches();
                    lastSelectedTile.DeselectTile();
                    ClearAllMatches();
                }
                else
                {
                    lastSelectedTile.GetComponent<Tile>().DeselectTile();
                    SelectTile();
                }
            }
        }
    }

    public void SpawnSprite(SpriteRenderer render2)
    {
        if (render.sprite == render2.sprite)
        {
            return;
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
        SFXManager.instance.PlaySFX(Clip.Swap);
    }

    private GameObject GetAjacentTile(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAjacentTile(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles;
    }

    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i <paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= matchRequirement)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }

    public void ClearAllMatches()
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
            StopCoroutine(MatchBoardManager.instance.FindNullTiles());
            StartCoroutine(MatchBoardManager.instance.FindNullTiles());
            SFXManager.instance.PlaySFX(Clip.Clear);
        }
    }
}
