using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;

public class Astar : MonoBehaviour
{
    public static Astar inst;

    private void Awake()
    {
        inst = this;
    }

    Node[,] nodes;
    public int nodeCountX, nodeCountY;
    public float xMin, xMax, yMin, yMax;


    public RaycastHit2D hit;
    int groundLayerMask;
    int obstacleLayerMask;

    public GameObject waypointPrefab;
    public GameObject startPointPrefab;
    public GameObject endPointPrefab;

    public Transform waypointParent;

    Vector3 startPointPosition;
    Vector3 endPointPosition;

    List<Node> frontier;
    HashSet<Node> explored;
    List<Node> smoothPath;

    public bool showNodes = false;
    public bool showWayPointPath = false;

    void Start()
    {
        frontier = new List<Node>();
        explored = new HashSet<Node>();

        groundLayerMask = 1 << 6;
        obstacleLayerMask = 1 << 3;

        nodes = new Node[nodeCountX, nodeCountY];
        // code from slides

        // generates grid of nodes
        for (int i = 0; i < nodeCountX; i++)
        {
            for (int j = 0; j < nodeCountY; j++)
            {
                nodes[i, j] = new Node();
                float xPos = xMin + i * (xMax - xMin) / (nodeCountX - 1);
                float yPos = yMin + j * (yMax - yMin) / (nodeCountY - 1);
                nodes[i, j].worldPosition = new Vector3(xPos, yPos, 0);
                nodes[i, j].gridPositionX = i;
                nodes[i, j].gridPositionY = j;

                // Check where all nodes are
                if (showNodes)
                    Instantiate(waypointPrefab, nodes[i,j].worldPosition, Quaternion.identity);

            }
        }
        //Debug.Log(nodes.Length);

        // Draws border of search space
        Debug.DrawLine(nodes[0, 0].worldPosition, nodes[nodeCountX - 1, 0].worldPosition, Color.white, float.MaxValue);
        Debug.DrawLine(nodes[nodeCountX - 1, 0].worldPosition, nodes[nodeCountX - 1, nodeCountY - 1].worldPosition, Color.white, float.MaxValue);
        Debug.DrawLine(nodes[nodeCountX - 1, nodeCountY - 1].worldPosition, nodes[0, nodeCountY - 1].worldPosition, Color.white, float.MaxValue);
        Debug.DrawLine(nodes[0, nodeCountY - 1].worldPosition, nodes[0, 0].worldPosition, Color.white, float.MaxValue);
    }

    void Update()
    {
        
        if (Selection.inst.selectedUnits.Count != 0)
        {
            startPointPosition = Selection.inst.selectedUnits[0].position;
            if (Input.GetMouseButtonDown(1))
            {

                //Debug.Log("Starting Position: " + Selection.inst.selectedUnits[0].position);


                //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                
                //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),  Vector2.zero, Mathf.Infinity, groundLayerMask);
                
                //Ray ray = new Ray(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
                //RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

                // https://forum.unity.com/threads/2d-raycast-in-z-direction.525010/
                Collider2D[] colliders = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                bool hitObstacle = false;
                bool hitGround = false;
                // loops through each collider hit
                foreach(Collider2D collider in colliders)
                {
                    //Debug.Log(collider);
                    // if mouse click hits an obstacle --> don't do anything
                    if (collider.gameObject.CompareTag("Obstacle"))
                    {
                        hitObstacle = true;
                        //Debug.Log("hit obstacle");
                        
                    }
                    // otherwise if mouse click hits the ground -- then do astar
                    else if (collider.gameObject.CompareTag("Ground"))
                    {
                        hitGround = true;
                        //Debug.Log("hit ground");

                    }
                        
                }

                if (!hitObstacle && hitGround)
                {
                    
                    //endPointPosition = hit.point;
                    endPointPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    //Debug.Log("Ending Position: " + endPointPosition);
                    //Debug.Log("Path generated");
                    GeneratePathWaypoints(FindPath());

                    if (smoothPath.Count > 0)
                    {
                        for (int i = 0; i < smoothPath.Count; i++)
                        {
                            //Debug.Log("Move to: " + smoothPath[i].worldPosition);
                            MoveToCheckpoint(smoothPath[i].worldPosition);
                        }

                    }
                }
            }
        }
    }

    public List<Node> FindPath()
    {

        /*
         * add startnode to frontier
         * while frontier is not empty
         *      list of nearby available nodes of currentnode = FindNearbyNodes()
         *      if list neaby available nodes is null
         *      
         *      loop through nearby available nodes
         *          currentnode.totalPath
         *          find f score of node
         *          keep track of lowest f score
         *      add currentnode to visited list
         *      
         *      currentnode is now the node with lowest f score
         *      add currentnode to frontier
         * 
         */


        // clear 
        frontier.Clear();
        explored.Clear();

        // find startNode and endNode based on user input
        Node startNode = FindNearestNode(startPointPosition);
        Node endNode = FindNearestNode(endPointPosition);

        // set up start node        
        startNode.gCost = 0;
        startNode.hCost = CalculateHCost(startNode, endNode);
        startNode.parentNode = null;
        CalculateFCost(startNode.parentNode, startNode, endNode);

        frontier.Add(startNode);


        while (frontier.Count > 0)
        {

            // if frontier is empty --> then no path was found
            if (frontier.Count == 0)
            {
                Debug.Log("Path not found");
                return null;
            }

            // current node
            Node node = frontier[0];

            // path is found when current node equals end node
            if (node == endNode)
            {
                return GeneratePath(node);
                //return path;
            }

            frontier.RemoveAt(0);
            explored.Add(node);

            // search nodes around current node
            foreach (Node nearbyNode in FindNearbyNodes(node))
            {
                //Debug.Log("checking nearby node");

                // if current node has not been explored or is not planned to be explored --> add to list
                if (!explored.Contains(nearbyNode) && !frontier.Contains(nearbyNode))
                {
                    //Debug.Log("New nearby node");

                    nearbyNode.parentNode = node;
                    nearbyNode.gCost = CalculateGCost(node, nearbyNode);
                    nearbyNode.hCost = CalculateHCost(nearbyNode, endNode);
                    CalculateFCost(node, nearbyNode, endNode);
                    PriorityInsert(nearbyNode);
                }
                // if visiting a node again --> check if it has a better gCost than before
                else if (frontier.Contains(nearbyNode))
                {
                    float potentialGValue = node.gCost + CalculateGCost(node, nearbyNode);
                    if (node.gCost + CalculateGCost(node, nearbyNode) > nearbyNode.gCost)
                    {
                        frontier.Remove(nearbyNode);

                        nearbyNode.parentNode = node;
                        nearbyNode.gCost = potentialGValue;
                        nearbyNode.hCost = CalculateHCost(nearbyNode, endNode);
                        CalculateFCost(node, nearbyNode, endNode);

                        // took out and reinserted node so it can be sorted properly into frontier w/ its new gCost
                        PriorityInsert(nearbyNode);
                    }
                }
            }
        }

        Debug.Log("No path!");
        return null;

    }

    // works backward from the end to find a path
    public List<Node> GeneratePath(Node endNode)
    {
        Stack<Node> path = new Stack<Node>();

        Node pathNode = endNode;
        while (pathNode != null)
        {
            path.Push(pathNode);
            pathNode = pathNode.parentNode;
        }

        return path.ToList(); // ToList() will reverse the stack so the path is in the correct order
    }

    public List<Node> FindNearbyNodes(Node node)
    {
        List<Node> nearbyNodes = new List<Node>();

        // goes through nodes in all eight directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // if its the current node then skip
                if (i == 0 && j == 0)
                    continue;


                int indexX = node.gridPositionX + i;
                int indexY = node.gridPositionY + j;

                // check if the indices are within bounds
                if (0 <= indexX && indexX <= nodeCountX - 1 &&
                    0 <= indexY && indexY <= nodeCountY - 1)
                {
                    Node newNode = nodes[indexX, indexY];

                    nearbyNodes.Add(newNode);
                    //Debug.DrawRay(node.worldPosition, new Vector2(i, j) * 10 , Color.magenta, float.MaxValue);
                    /*
                    if (Physics2D.Raycast(node.worldPosition,
                        new Vector3(i, j, 0),
                        Vector3.Magnitude(newNode.worldPosition - node.worldPosition), obstacleLayerMask))
                    {

                        nearbyNodes.Remove(newNode);
                    }
                     */
                    Vector2 vectorDiff = newNode.worldPosition - node.worldPosition;
                    float distance = vectorDiff.magnitude;

                    

                    RaycastHit2D hit = Physics2D.Raycast(node.worldPosition, new Vector2(i, j), distance, obstacleLayerMask);

                    if (hit.collider != null)
                    {
                        nearbyNodes.Remove(newNode);
                    }
                }
            }
        }
        
        // Check how nodes are expanded
        if (showWayPointPath)
        {
            foreach (Node nearbyNode in nearbyNodes)
            {
                Instantiate(waypointPrefab, nearbyNode.worldPosition, Quaternion.identity);
            }
        }
        
        
        //Debug.Log($"Amount of available nodes: {nearbyNodes.Count}");
        return nearbyNodes;
    }

    public void PriorityInsert(Node node) // uses frontier queue
    {
        int insertIndex = 0;

        // keeps track of index until there is an fCost larger than the node's fCost
        for (int i = 0; i < frontier.Count; i++)
        {
            if (frontier[i].fCost <= node.fCost)
            {
                insertIndex = i + 1;
            }
        }

        frontier.Insert(insertIndex, node);
    }

    public void ClearWaypoints()
    {
        foreach (Transform waypoint in waypointParent.transform)
        {
            Destroy(waypoint.gameObject);
        }
    }

    public List<Node> GeneratePathWaypoints(List<Node> path)
    {
        

        // filter through path and find the waypoints
        // display waypoints
        ClearWaypoints();
        if (path == null)
        {
            Debug.Log("path is null");
        }
        else
        {
            
            // BUG: Smooth Path not working -- Physics2D.Linecast is not detecting obstacles

            smoothPath = new List<Node>();
            int lastIndex = path.Count - 1;

            smoothPath.Add(path[0]);

            for (int i = 1; i < path.Count - 1; i++)
            {
                int smoothLastIndex = smoothPath.Count - 1;

                if (Physics2D.Linecast(smoothPath[smoothLastIndex].worldPosition, path[i + 1].worldPosition, obstacleLayerMask))
                {
                    smoothPath.Add(path[i]);
                }
            }

            smoothPath.Add(path[lastIndex]);
            
        }
        return null;
    }
    public void MoveToCheckpoint(Vector3 newPosition)
    {
        foreach (Unit unit in Selection.inst.selectedUnits)
        {
            //Debug.Log(unit);
            Move m = new Move(unit, newPosition);
            UnitAI ai = unit.GetComponent<UnitAI>();
            ai.StopAndRemoveAllCommands();
            ai.AddCommand(m);

        }

    }
    public Node FindNearestNode(Vector3 position) // Does not account for spawning in obstacles
    {
        //float xPos = xMin + i * (xMax - xMin) / (nodeCountX - 1);
        //float yPos = yMin + j * (yMax - yMin) / (nodeCountY - 1);

        // Uses the above equations to find i and j (node indices)
        int nodeX = Mathf.RoundToInt((position.x - xMin) / ((xMax - xMin) / (nodeCountX - 1)));
        int nodeY = Mathf.RoundToInt((position.y - yMin) / ((yMax - yMin) / (nodeCountY - 1)));

        Node nearestNode = nodes[nodeX, nodeY];

        //Instantiate(waypointPrefab, nearestNode.worldPosition, Quaternion.identity);

        return nearestNode;
    }

    public float CalculateHCost(Node currentNode, Node goalNode)
    {
        // Uses Manhattan Distance (number of nodes traversed)
        float manhattanDistance = Mathf.Abs(currentNode.gridPositionX - goalNode.gridPositionX) + Mathf.Abs(currentNode.gridPositionY - goalNode.gridPositionY);

        //Debug.Log("ManhattanDistance: " + manhattanDistance);

        return manhattanDistance;
    }

    public float CalculateGCost(Node parentNode, Node currentNode)
    {
        // distance formula
        float distance = Vector3.Magnitude(currentNode.worldPosition - parentNode.worldPosition);

        return distance + parentNode.gCost;
    }

    public void CalculateFCost(Node parentNode, Node currentNode, Node goalNode)
    {
        if (parentNode == null)
        {
            currentNode.gCost = 0;
            currentNode.hCost = CalculateHCost(currentNode, goalNode);
            currentNode.fCost = currentNode.hCost;

            return;
        }

        float gValue = CalculateGCost(parentNode, goalNode);

        float hValue = CalculateHCost(currentNode, goalNode);

        float fValue = gValue + hValue;
        currentNode.fCost = fValue;

    }
}
