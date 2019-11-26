using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform one;
    public Transform two;
    public Transform three;
    List<Vector3> path;
    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        path = new List<Vector3>();
        path.Add(one.position);
        path.Add(two.position);
        path.Add(three.position);
        Move();
    }

    void Move()
    {
        if(i < path.Count)
        {
            transform.DOMove(path[i], 1).OnComplete(() =>
            {
                ++i;
                Move();
            });
        }
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnMouseDown()
    {
        Debug.Log("DOwn");
    }

    private void OnMouseEnter()
    {
        Debug.Log("En");
    }

    private void OnMouseExit()
    {
        Debug.Log("Ex");
    }

    private void OnMouseOver()
    {
        Debug.Log("O");
    }
}
