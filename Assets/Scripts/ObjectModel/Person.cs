using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person
{
    public int Id { get; set; } //编号，图片素材命名用编号
    public Vector2Int FightStartRowCol { get; set; } //人物在战斗场景中的几行几列
    public GameObject PersonObject { get; set; } //在Scene中的GameObject
}
