using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject gridPrefab; //格子的预制体
    public GameObject personPrefab; //人物的预制体
    public static Dictionary<GameObject, Vector2Int> gridUnitsObjecToData; //通过格子对象获取格子的行列
    public static Dictionary<Vector2Int, GameObject> gridUnitsDataToObject; //通过格子的行列获取格子对象
    public static Dictionary<Vector3, Person> positionToFriend; //通过友方在战斗场景中的位置获取友方实例
    public static Dictionary<Vector3, Person> positionToEnemy; //通过友方在战斗场景中的位置获取友方实例
    public static int mapWidth = 12; //地图的最大列数
    public static int mapHeight = 9; //地图的最大行数

    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
        GlobalData.FriendQueue.Add(0);
        GlobalData.EnemyQueue.Add(1);
        GlobalData.EnemyQueue.Add(2);
        Person test = new Person
        {
            Id = 0,
            FightStartRowCol = new Vector2Int(4, 4)
        };
        Person enemy1 = new Person
        {
            Id = 1,
            FightStartRowCol = new Vector2Int(3, 4)
        };
        Person enemy2 = new Person
        {
            Id = 2,
            FightStartRowCol = new Vector2Int(5, 4)
        };
        GlobalData.Persons.Add(0, test);
        GlobalData.Persons.Add(1, enemy1);
        GlobalData.Persons.Add(2, enemy2);
        positionToFriend = new Dictionary<Vector3, Person>(); 
        positionToEnemy = new Dictionary<Vector3, Person>();
        SetFightPerson(GlobalData.FriendQueue, positionToFriend);
        SetFightPerson(GlobalData.EnemyQueue, positionToEnemy);
    }

    void CreateMap()
    {
        float width = gridPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        float height = gridPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        gridUnitsObjecToData = new Dictionary<GameObject, Vector2Int>();
        gridUnitsDataToObject = new Dictionary<Vector2Int, GameObject>();
        for (int j = 0; j < mapHeight; ++j)
        {
            int realMapWidth = j % 2 == 0 ? mapWidth : mapWidth - 1;
            for (int i = 0; i < realMapWidth; ++i)
            {
                GameObject newGrid = Instantiate(gridPrefab);
                newGrid.name = j + "_" + i;
                newGrid.transform.parent = transform;
                float x = j % 2 == 0 ? i : i + 0.5f;
                newGrid.transform.position += new Vector3(x * width, -j * height * 0.75f, 0);
                var rowAndCol = new Vector2Int(j, i);
                gridUnitsObjecToData.Add(newGrid, rowAndCol);
                gridUnitsDataToObject.Add(rowAndCol, newGrid);
            }
        }
        gameObject.transform.Rotate(30, 0, 0);
    }

    void SetFightPerson(List<int> personQueue, Dictionary<Vector3, Person> reflect)
    {
        foreach(var id in personQueue)
        {
            Person person = GlobalData.Persons[id];
            Vector3 personPosition = gridUnitsDataToObject[person.FightStartRowCol].transform.position;
            GameObject personObject = Instantiate(personPrefab, personPosition, Quaternion.identity);
            personObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("head/" + 0);
            person.PersonObject = personObject;
            reflect.Add(personPosition, person);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
