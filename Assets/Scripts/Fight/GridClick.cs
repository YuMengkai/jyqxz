using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridClick : MonoBehaviour
{
    public enum BattleControlState { Empty, Rest, Moving, Moved};

    private Color defaultColor;
    private Color pathColor;
    private Color rangeColor;
    private readonly float speed = 0.2f;
    private static GameObject battleControlObject;
    public static BattleControlState controlState = BattleControlState.Empty;
    private static HashSet<Vector2Int> moveRangeGrids = new HashSet<Vector2Int>();
    private static GameObject currentPersonObject;
    private List<Vector2Int> movePath = new List<Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        defaultColor = gameObject.GetComponent<SpriteRenderer>().color;
        pathColor = new Color(148, 0, 211, defaultColor.a);
        rangeColor = new Color(0, 191, 255, defaultColor.a);
        if (gameObject.name.Equals("0_0"))
        {
            battleControlObject = GameObject.Find("battleControlBlock");
            battleControlObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if(currentPersonObject != null)
        {
            switch (controlState)
            {
                case BattleControlState.Moving:
                    if (moveRangeGrids.Contains(Main.gridUnitsObjecToData[gameObject]))
                    {
                        currentPersonObject.GetComponent<SpriteRenderer>().color = defaultColor;
                        controlState = BattleControlState.Moved;
                        MovePerson();
                        ShowPath(defaultColor);
                        ShowMoveRange(defaultColor);
                    }
                    break;
            }
        }
        else
        {
            if (Main.positionToFriend.ContainsKey(gameObject.transform.position) && !controlState.Equals(BattleControlState.Moved))
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, defaultColor.a);
                controlState = BattleControlState.Moving;
                battleControlObject.SetActive(true);
                currentPersonObject = gameObject;
                GenerateMoveRange(2);
                ShowMoveRange(rangeColor);
            }
        }
    }

    private void OnMouseEnter()
    {
        if(moveRangeGrids.Contains(Main.gridUnitsObjecToData[gameObject]) && 
            controlState == BattleControlState.Moving)
        {
            movePath = FindPath(Main.gridUnitsObjecToData[gameObject]);
            ShowPath(pathColor);
        }
    }

    private void OnMouseExit()
    {
        if (controlState == BattleControlState.Moving)
        {
            ShowPath(rangeColor);
        }
    }

    private void GenerateMoveRange(int rank)
    {
        Vector2Int startPosition = Main.gridUnitsObjecToData[gameObject];
        var preRange = new Vector2Int();

        for (int i = 1; i <= rank; ++i)
        {
            var rc = new Vector2Int(startPosition.x, Mathf.Min(startPosition.y + i,
                startPosition.x % 2 == 0 ? Main.mapWidth - 1 : Main.mapWidth - 2));
            if(FindPath(rc).Count <= rank)
            {
                moveRangeGrids.Add(rc);
            }
            preRange.y = startPosition.y + i;
        }
        for (int i = 1; i <= rank; ++i)
        {
            var rc = new Vector2Int(startPosition.x, Mathf.Max(startPosition.y - i, 0));
            if (FindPath(rc).Count <= rank)
            {
                moveRangeGrids.Add(rc);
            }
            preRange.x = startPosition.y - i;
        }
        for (int i = 1; i <= rank; ++i)
        {
            var currentRange = new Vector2Int();
            if (startPosition.x % 2 != 0)
            {
                if (i % 2 != 0)
                {
                    currentRange.x = preRange.x + 1;
                    currentRange.y = preRange.y;
                }
                else
                {
                    currentRange.x = preRange.x;
                    currentRange.y = preRange.y - 1;
                }
            }
            else
            {
                if (i % 2 != 0)
                {
                    currentRange.x = preRange.x;
                    currentRange.y = preRange.y - 1;
                }
                else
                {
                    currentRange.x = preRange.x + 1;
                    currentRange.y = preRange.y;
                }
            }
            preRange = currentRange;
            for (int j = currentRange.x; j <= currentRange.y; ++j)
            {
                if(startPosition.x + i <= Main.mapHeight - 1)
                {
                    int maxWidth = (startPosition.x + i) % 2 == 0 ? Main.mapWidth - 1 : Main.mapWidth - 2;
                    if(j >= 0 && j <= maxWidth)
                    {
                        var rc = new Vector2Int(startPosition.x + i, j);
                        var gridPosition = Main.gridUnitsDataToObject[rc].transform.position;
                        if (!Main.positionToEnemy.ContainsKey(gridPosition) && FindPath(rc).Count <= rank)
                        {
                            moveRangeGrids.Add(rc);
                        }
                    }
                }
                if (startPosition.x - i >= 0)
                {
                    int maxWidth = (startPosition.x - i) % 2 == 0 ? Main.mapWidth - 1 : Main.mapWidth - 2;
                    if (j >= 0 && j <= maxWidth)
                    {
                        var rc = new Vector2Int(startPosition.x - i, j);
                        var gridPosition = Main.gridUnitsDataToObject[rc].transform.position;
                        if (!Main.positionToEnemy.ContainsKey(gridPosition) && FindPath(rc).Count <= rank)
                        {
                            moveRangeGrids.Add(rc);
                        }
                    }
                }
            }
        }
    }

    void ShowMoveRange(Color color)
    {
        foreach (var point in moveRangeGrids)
        {
            if (point.Equals(Main.gridUnitsObjecToData[gameObject]))
            {
                continue;
            }
            var gridObject = Main.gridUnitsDataToObject[point];
            gridObject.GetComponent<SpriteRenderer>().color = color;
        }
    }

    private List<Vector2Int> FindPath(Vector2Int endPosition)
    {
        Vector2Int startPosition = Main.gridUnitsObjecToData[currentPersonObject];

        HashSet<Vector2Int> enemyRc = new HashSet<Vector2Int>();
        foreach(var id in GlobalData.EnemyQueue)
        {
            enemyRc.Add(GlobalData.Persons[id].FightStartRowCol);
        }
        HashSet<Vector2Int> grids = new HashSet<Vector2Int>();
        foreach(var key in Main.gridUnitsDataToObject.Keys)
        {
            grids.Add(key);
        }
        var path = PathFinding2D.Astar(startPosition, endPosition, grids, enemyRc);
        path.Remove(startPosition);
        return path;
    }

    void ShowPath(Color color)
    {
        foreach(var point in movePath)
        {
            var gridObject = Main.gridUnitsDataToObject[point];
            gridObject.GetComponent<SpriteRenderer>().color = color;
        }
    }

    void RestoreGrid()
    {
        foreach (var point in movePath)
        {
            var gridObject = Main.gridUnitsDataToObject[point];
            gridObject.GetComponent<SpriteRenderer>().color = defaultColor;
        }
    }

    void MovePerson()
    {
        List<Vector3> realPath = new List<Vector3>();
        foreach (var point in movePath)
        {
            var gridObject = Main.gridUnitsDataToObject[point];
            realPath.Add(gridObject.transform.position);
        }
        Move(Main.positionToFriend[currentPersonObject.transform.position].PersonObject, realPath);
    }

    static int pathIndex = 0;
    void Move(GameObject personObject, List<Vector3> path)
    {
        if (pathIndex < path.Count)
        {
            personObject.transform.DOMove(path[pathIndex], speed).OnComplete(() =>
            {
                ++pathIndex;
                Move(personObject, path);
            });
        }
    }
}
