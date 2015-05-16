using UnityEngine;
using System.Collections;

public class Pawn : Unit
{
    //Unit attributes
    float moveSpeed = 0.5f;

    ArrayList route = new ArrayList();

    class pathNode
    {
        public pathNode previous;
        public Vector3 pos;
        public int cumulativeCost;
    };

    void Update()
    {
        //code to check whether the current order is done and take on the next order from the queue if so
        if (route.Count <= 0)
            return;

        //move toward current target node
        Vector3 pos = currentPosition();
        Vector3 dest = (Vector3)route[0];

        Vector3.MoveTowards(pos, dest, moveSpeed * Time.deltaTime);
    }

    void pathFind(Vector3 finish)
    {
        //must maintain priority queue format (lowest cost first)
        ArrayList open = new ArrayList();

        ArrayList closed = new ArrayList();

        //implementation of A* pathfinding algo
        pathNode start = new pathNode();
        start.pos = currentPosition();
        start.previous = null;
        start.cumulativeCost = heuristic(start, finish);
        open.Add(start);
        while (open.Count > 0) //there are open nodes
        {
            while (!((pathNode)open[0]).pos.Equals(finish)) //pos is not equal to finish
            {
                //pop the first node from the open list
                pathNode current = (pathNode)open[0];
                open.RemoveAt(0);
            }
        }
        //can pathfind along blocks that are equal height or plus or minus one level of elevation
        //and that have two blocks' height open above them
        //going up elevation has 2x weight
        //going down elevation has 0.5x weight
    }

    Vector3 currentPosition()
    {
        Vector3 pos = new Vector3();
        pos.x = (int)transform.position.x;
        pos.y = (int)transform.position.y;
        pos.z = (int)transform.position.z;
        return pos;
    }

    //admissible heuristic for 8 directions of movement
    int heuristic(pathNode node, Vector3 finish)
    {
        int deltaX = (int)Mathf.Abs(node.pos.x - finish.x);
        int deltaZ = (int)Mathf.Abs(node.pos.z - finish.z);

        return Mathf.Max(deltaX, deltaZ);
    }

    void priorityInsert(pathNode node, ArrayList list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (node.cumulativeCost < ((pathNode)list[i]).cumulativeCost)
            {
                list.Insert(i, node);
                break;
            }
        }
    }

    bool isConnected(pathNode start, pathNode finish)
    {
        //implement code to test for connectivity between start and finish
        return true;
    }

    float moveCost(float baseCost, pathNode from, pathNode to)
    {
        if (from.pos.y - to.pos.y == 1)
            return baseCost * 2;

        if (from.pos.y - to.pos.y == -1)
            return baseCost * 0.5f;

        if (from.pos.y - to.pos.y == 0)
            return baseCost;

        return -1; //elevation change beyond acceptable limit
    }
}
