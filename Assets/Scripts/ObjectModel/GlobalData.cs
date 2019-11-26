using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData
{
    public static Dictionary<int, Person> Persons { get; set; } //所有人物实例，键是id
    public static List<int> FriendQueue { get; set; }//友方战斗的人id
    public static List<int> EnemyQueue { get; set; }//敌方战斗的人id

    static GlobalData()
    {
        Persons = new Dictionary<int, Person>();
        FriendQueue = new List<int>();
        EnemyQueue = new List<int>();
    }
}
