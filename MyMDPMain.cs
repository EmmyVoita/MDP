using System.Collections;
using UnityEngine;

public class MyMDPMain : MonoBehaviour
{
    public GameObject GridMarker;
    public const int GRID_SIZE = 4;
    public float discountFactor = 0.99f;
    public float threshold = 0.001f;
    public int maxIterations = 100;
    // Define the grid size
    

    // Define the obstacle positions
    private readonly Vector2Int[] OBSTACLE_POSITIONS = {
           new Vector2Int(0, 2),
           new Vector2Int(2, 1),
           new Vector2Int(3, 3),
           //new Vector2Int(6, 2),
           //new Vector2Int(5, 5)
    };

    // Define the reward values for each state
    [SerializeField]
    public float[,] REWARDS = {
        //Reads 0 ->
        {-1,  0, 0, 0 },
        // ->
        { 0, 10, 1, 0 },
        // ->
        {-1,  1, 0, 0 },
        // -> 15
        { 0,  0, 0, 0 }, 
    };

    // Define the MDP
    private MyMDP mdp;


    // Define the current state of the object in the MDP
    private int currentState;

    void Start()
    {

        ShowGrid();

        // Initialize the MDP
        mdp = new MyMDP(GRID_SIZE, discountFactor, REWARDS);

        // Set the obstacle states
        foreach (Vector2Int obstaclePos in OBSTACLE_POSITIONS)
        {
            int obstacleState = mdp.GetState(obstaclePos.x, obstaclePos.y);
            mdp.SetObstacleState(obstacleState);
        }

       

        /*for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                Debug.Log(REWARDS[3-j, i]);
            }
        }*/


        // Run value iteration to get the optimal value function
        float[] valueFunction = mdp.ValueIteration(threshold, maxIterations);

        // Get the optimal policy
        int[] policy = mdp.GetOptimalPolicy(valueFunction);

        // Print the value function and policy
        Debug.Log("Value function:");
        for (int i = 0; i < valueFunction.Length; i++)
        {
            Debug.Log("State " + i + " x:" + mdp.GetX(i)+ " y:" + mdp.GetY(i) + " Value: " + valueFunction[i]);
        }

        Debug.Log("Optimal policy:");
        for (int i = 0; i < policy.Length; i++)
        {
            Debug.Log("State " + i + ": " + policy[i]);
        }

        Mover mover = gameObject.AddComponent<Mover>();
        mover.SetUp(mdp, policy);
    }


    void ShowGrid()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                GameObject marker = Instantiate(GridMarker, new Vector2(i,j), Quaternion.identity);
                for (int k = 0; k < OBSTACLE_POSITIONS.Length; k++)
                {
                    if(marker.transform.position == new Vector3(OBSTACLE_POSITIONS[k].x, OBSTACLE_POSITIONS[k].y,0))
                    {
                        marker.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
            }
        }
        
    }
}


