using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public static class PathFinding2D
{

    static float GetDistance(Vector2Int a, Vector2Int b)
    {
        float xDistance = Mathf.Abs(a.x - b.x);
        float yDistance = Mathf.Abs(a.y - b.y) * Mathf.Sqrt(6);
        return xDistance * xDistance + yDistance * yDistance;
    }

    static List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        var neighbors = new List<Vector2Int>();
        if (pos.x % 2 == 0)
        {
            neighbors.Add(new Vector2Int(pos.x + 1, pos.y));
            neighbors.Add(new Vector2Int(pos.x - 1, pos.y));
            neighbors.Add(new Vector2Int(pos.x + 1, pos.y - 1));
            neighbors.Add(new Vector2Int(pos.x - 1, pos.y - 1));
            neighbors.Add(new Vector2Int(pos.x, pos.y + 1));
            neighbors.Add(new Vector2Int(pos.x, pos.y - 1));
        }
        else
        {
            neighbors.Add(new Vector2Int(pos.x + 1, pos.y));
            neighbors.Add(new Vector2Int(pos.x - 1, pos.y));
            neighbors.Add(new Vector2Int(pos.x + 1, pos.y + 1));
            neighbors.Add(new Vector2Int(pos.x - 1, pos.y + 1));
            neighbors.Add(new Vector2Int(pos.x, pos.y + 1));
            neighbors.Add(new Vector2Int(pos.x, pos.y - 1));
        }
        return neighbors;
    }


    public static List<Vector2Int> Astar(Vector2Int from, Vector2Int to, HashSet<Vector2Int> map, HashSet<Vector2Int> impassableValues)
    {
        var result = new List<Vector2Int>();
        if (from == to)
        {
            result.Add(from);
            return result;
        }
        Node finalNode;
        List<Node> open = new List<Node>();
        if (FindDest(new Node(null, from, GetDistance(from, to), 0), open, map, to, out finalNode, impassableValues))
        {
            while (finalNode != null)
            {
                result.Add(finalNode.pos);
                finalNode = finalNode.preNode;
            } 
        }
        result.Reverse();
        return result;
    }

    static bool FindDest(Node currentNode, List<Node> openList,
                         HashSet<Vector2Int> map, Vector2Int to, out Node finalNode, HashSet<Vector2Int> impassableValues)
    {
        if (currentNode == null) {
            finalNode = null;
            return false;
        }
        else if (currentNode.pos == to)
        {
            finalNode = currentNode;
            return true;
        }
        currentNode.open = false;
        openList.Add(currentNode);

        foreach (var item in GetNeighbors(currentNode.pos))
        {
            if (map.Contains(item) && !impassableValues.Contains(item))
            {
                FindTemp(openList, currentNode, item, to);
            }
        }
        var next = openList.FindAll(obj => obj.open).Min();
        return FindDest(next, openList, map, to, out finalNode, impassableValues);
    }

    static void FindTemp(List<Node> openList, Node currentNode, Vector2Int from, Vector2Int to)
    {

        Node temp = openList.Find(obj => obj.pos == (from));
        if (temp == null)
        {
            temp = new Node(currentNode, from, GetDistance(from, to), currentNode.gScore + 1);
            openList.Add(temp);
        }
        else if (temp.open && temp.gScore > currentNode.gScore + 1)
        {
            temp.gScore = currentNode.gScore + 1;
            temp.fScore = temp.hScore + temp.gScore;
            temp.preNode = currentNode;
        }
    }

    class Node:IComparable
    {
        public Node preNode;
        public Vector2Int pos;
        public float fScore;
        public float hScore;
        public float gScore;
        public bool open = true;

        public Node(Node prePos, Vector2Int pos, float hScore, float gScore)
        {
            this.preNode = prePos;
            this.pos = pos;
            this.hScore = hScore;
            this.gScore = gScore;
            this.fScore = hScore + gScore;
        }

        public int CompareTo(object obj)
        {
            Node temp = obj as Node;

            if (temp == null) return 1;

            if (Mathf.Abs(this.fScore - temp.fScore) < 0.01f) {
                return this.fScore > temp.fScore ? 1 : -1;
            }

            if (Mathf.Abs(this.hScore - temp.hScore) < 0.01f)
            {
                return this.hScore > temp.hScore ? 1 : -1;
            }
            return 0;
        }
    }
}