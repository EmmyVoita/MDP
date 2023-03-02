using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Mover : MonoBehaviour
{
    // The speed at which the object moves
    public float speed = 1f;

    // The position of the object in the grid
    private Vector2Int position;

    // The MDP object
    private MyMDP mdp;

    // The policy for moving the object
    private int[] policy;

    bool canMove = true;

    private int currentState;

    // Set up the Mover
    public void SetUp(MyMDP mdp, int[] policy)
    {
        this.mdp = mdp;
        this.policy = policy;

        // Get the starting position of the object
        int startState = mdp.GetState(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        //currentState = mdp.GetState(0, 0);
        position = new Vector2Int(mdp.GetX(startState), mdp.GetY(startState));
        //transform.position = new Vector3(0, 0, 0);
        //StartCoroutine(Move());
    }

   /* private IEnumerator Move()
    {
        while (true)
        {
            int action = policy[currentState];
            int nextState = mdp.GetNextState(currentState, action);
            if (mdp.IsObstacleState(nextState))
            {
                // If the next state is an obstacle, skip it
                Debug.Log("Skipping obstacle state " + nextState);
                nextState = mdp.GetNextState(nextState, action);
            }
            Vector2Int nextPos = mdp.GetPosition(nextState);
            Vector3 nextPos3D = new Vector3(nextPos.x, nextPos.y, 0);
            while (transform.position != nextPos3D)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPos3D, Time.deltaTime);
                yield return null;
            }
            currentState = nextState;
            if (mdp.IsTerminalState(currentState))
            {
                Debug.Log("Reached goal state " + currentState);
                yield break;
            }
        }
    }*/

    void Update()
    {
        // Move the object according to the policy
        int action = policy[mdp.GetState(position.x, position.y)];
        MoveObject(action);

    }

    // Move the object in the given direction
    private void MoveObject(int action)
    {
        // Get the next position based on the action
        int nextState = mdp.GetNextState(position.x, position.y, action);
        int nextX = mdp.GetX(nextState);
        int nextY = mdp.GetY(nextState);

        // Move the object to the next position
        Vector2 nextPos = new Vector2(nextX, nextY);
        transform.position = Vector2.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        //Debug.Log(nextX + " : " + nextY);

        // Pause the object for 0.3 seconds when it reaches the next position
        if ((Vector2)transform.position == nextPos && canMove == true)
        {
            canMove = false;
            StartCoroutine(PauseObject(nextX, nextY));
        }
    }
    // Coroutine to pause the object for 0.3 seconds
    private IEnumerator PauseObject(int nextX, int nextY)
    {
        Debug.Log("Reward = " + mdp.GetReward(nextX, nextY));
        yield return new WaitForSeconds(0.3f);
        // Update the current position of the object
        position = new Vector2Int(nextX, nextY);
        canMove = true;
    }
}
