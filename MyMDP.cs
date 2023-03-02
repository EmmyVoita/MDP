using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MyMDP
{
    // Grid size
    private int gridSize;

    // Number of states
    private int numStates;

    // Number of actions
    private int numActions;

    // Obstacle states
    private HashSet<int> obstacleStates;

    // State transitions and rewards

    // 3D array that stores the probabilities of transitioning from one state to another state under a given action.
    // Reads as: current state, action taken in the current state, the next state resulting from the action
    // The values in the "transitionProbs" should sum to 1.0 accross possible next states for a given state-action
    private float[,,] transitionProbs;
    private float[,,] rewards;
    private float[,] INPUT_REWARDS_TABLE;

    private float gamma;

    // Constructor
    public MyMDP(int gridSize, float gamma, float[,] REWARDS)
    {
        this.gridSize = gridSize;
        numStates = gridSize * gridSize;
        numActions = 4; // up, down, left, right
        obstacleStates = new HashSet<int>();
        transitionProbs = new float[numStates, numActions, numStates];
        rewards = new float[numStates, numActions, numStates];
        this.gamma = gamma;
        INPUT_REWARDS_TABLE = REWARDS;


        SetTransitionProbs();
        SetRewardsProbs();

    }

    public void SetTransitionProbs()
    {

        //Initialize everything to 0
        for (int state = 0; state < numStates; state++)
        {
            for (int action = 0; action < numActions; action++)
            {
                for (int nextState = 0; nextState < numStates; nextState++)
                {
                    transitionProbs[state, action, nextState] = 0f;
                }
            }
        }


        for (int state = 0; state < numStates; state++)
        {
            int x = GetX(state);
            int y = GetY(state);

            //Debug.Log("x: " + x + "y: " + y);

            // Check if the current state is an obstacle state
            if (IsObstacleState(state))
            {
                // If it is, set the transition probabilities to zero for all actions
                for (int action = 0; action < numActions; action++)
                {
                    for (int nextX = 0; nextX < gridSize; nextX++)
                    {
                        for (int nextY = 0; nextY < gridSize; nextY++)
                        {
                            int nextState = GetState(nextX, nextY);
                            transitionProbs[state, action, nextState] = 0f;
                        }
                    }
                }
                continue;
            }

            // Set the transition probabilities for each action
            for (int action = 0; action < numActions; action++)
            {
                int nextX = x;
                int nextY = y;
                int nextState = -1;

                // Move up if it would not take out of grid boundaries
                if (action == 0 && y < gridSize - 1)
                {
                    nextY += 1;
                    nextState = GetState(nextX, nextY);
                }
                // Move down if it would not take out of grid boundaries
                else if (action == 1 && y > 0)
                {
                    nextY -= 1;
                    nextState = GetState(nextX, nextY);
                }
                // Move left if it would not take out of grid boundaries
                else if (action == 2 && x > 0)
                {
                    nextX -= 1;
                    nextState = GetState(nextX, nextY);
                }
                // Move rightit if it would not take out of grid boundaries
                else if (action == 3 && x < gridSize - 1)
                {
                    nextX += 1;
                    nextState = GetState(nextX, nextY);
                }

                // Check if the movement would result in an obstacle or take the object outside the grid
                if (nextState == -1 || IsObstacleState(nextState))
                {
                    //if it takes it out of bounds, taking that action should result it it moving back to the same sate. 
                    if (nextState == -1)
                    {
                        transitionProbs[state, action, state] = 1f;
                        //transitionProbs[state, action, nextState] = 0f;
                    }
                    else if (IsObstacleState(nextState))
                    {
                        transitionProbs[state, action, nextState] = 0f;
                    }
                    //transitionProbs[state, action, nextState] = 0f;
                }
                else
                {
                    // Set the transition probability to 1 for this state-action pair
                    transitionProbs[state, action, nextState] = 1.0f;

                    // Set the transition probabilities to 0.1 for the other state-action pairs
                    /*for (int a = 0; a < numActions; a++)
                    {
                        if (a != action)
                        {
                            transitionProbs[state, a, nextState] = 0.1f;
                        }
                    }*/
                }
            }
        }
    }

    public void SetRewardsProbs()
    {
        // Calculate the transition probabilities and rewards for each state-action pair
        for (int state = 0; state < GetNumStates(); state++)
        {
            for (int action = 0; action < GetNumActions(); action++)
            {
                //Get the next state and the corresponding probability
                (int nextState, float prob) = GetNextStateAndProb(state, action);

                //If the probability of transitioning to that state is greater than 0, then set the reward for that state action-pair

                if (prob > 0)
                {
                    //get the reward associated with the nextState

                    float reward = INPUT_REWARDS_TABLE[(gridSize - 1) - GetY(nextState), GetX(nextState)];

                    //Debug.Log($"State: {state}, StateX: {GetX(state)}, StateY: {GetY(state)}, NextState: {nextState}, StateX: {GetX(nextState)}, StateY: {GetY(nextState)}, Action: {action}, Reward: {reward}");
                    SetReward(state, action, reward);
                }
            }
        }
    }

    // Get the state ID for a given position on the grid
    public int GetState(int x, int y)
    {
        return x * gridSize + y;
    }

    // Set a state as an obstacle state
    public void SetObstacleState(int state)
    {
        obstacleStates.Add(state);
    }

    // Check if a state is an obstacle state
    public bool IsObstacleState(int state)
    {
        return obstacleStates.Contains(state);
    }

    // Get the number of states in the MDP
    public int GetNumStates()
    {
        return numStates;
    }

    // Get the number of actions in the MDP
    public int GetNumActions()
    {
        return numActions;
    }

    // Get the next state and the corresponding probability for a given state-action pair
    public (int, float) GetNextStateAndProb(int state, int action)
    {
        int x = GetX(state);
        int y = GetY(state);

        // Determine the next position based on the action
        int newX = x;
        int newY = y;

        if (action == 0) // up
        {
            newY += 1;
        }
        else if (action == 1) // down
        {
            newY -= 1;
        }
        else if (action == 2) // left
        {
            newX -= 1;
        }
        else if (action == 3) // right
        {
            newX += 1;
        }

        // Check if the next position is within the bounds of the grid
        if (newX < 0 || newX >= gridSize || newY < 0 || newY >= gridSize)
        {
            return (state, 1f); // stay in the same state with probability 1
        }

        // Get the next state ID
        int nextState = GetState(newX, newY);

        // Get the probability of transitioning to the next state
        float prob = transitionProbs[state, action, nextState];

        return (nextState, prob);
    }

    // Get the next state for a given state-action pair
    public int GetNextState(int x, int y, int action)
    {
        int nextX = x;
        int nextY = y;

        // Move up
        if (action == 0 && y < gridSize - 1)
        {
            nextY += 1;
        }
        // Move down
        else if (action == 1 && y > 0)
        {
            nextY -= 1;
        }
        // Move left
        else if (action == 2 && x > 0)
        {
            nextX -= 1;
        }
        // Move right
        else if (action == 3 && x < gridSize - 1)
        {
            nextX += 1;
        }

        int nextState = GetState(nextX, nextY);

        // If the next state is an obstacle state, stay in the current state
        if (IsObstacleState(nextState))
        {
            nextState = GetState(x, y);
        }

        return nextState;
    }

    // Get the x-coordinate of a state
    public int GetX(int state)
    {
        return state / gridSize;
    }

    // Get the y-coordinate of a state
    public int GetY(int state)
    {
        return state % gridSize;
    }

    public void SetReward(int state, int action, float reward)
    {
        /*if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
        {
            int state = GetState(x, y);
            for (int a = 0; a < numActions; a++)
            {
                for (int s = 0; s < numStates; s++)
                {
                    rewards[state, a, s] = reward;
                }
            }
        }*/

        rewards[state, action, GetNextState(GetX(state), GetY(state), action)] = reward;
        //Debug.Log($"State: {state}, Action: {action}, GetNextState: {GetNextState(GetX(state), GetY(state), action)}, Reward: {reward}");
    }

    public float GetReward(int x, int y)
    {
        if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
        {
            int state = GetState(x, y);
            float reward = 0f;
            for (int a = 0; a < numActions; a++)
            {
                for (int s = 0; s < numStates; s++)
                {
                    reward += GetTransitionProb(state, a, s) * rewards[state, a, s];
                }
            }
            return reward;
        }
        return float.MinValue; // Invalid reward
    }

    public void SetTransitionProb(int state, int action, int nextState, float prob)
    {
        transitionProbs[state, action, nextState] = prob;
    }

    public float GetTransitionProb(int state, int action, int nextState)
    {
        return transitionProbs[state, action, nextState];
    }

    public float[] GetActionValues(int state, float[] valueFunction)
    {
        float[] actionValues = new float[numActions];
        for (int a = 0; a < numActions; a++)
        {
            float value = 0f;
            for (int s = 0; s < numStates; s++)
            {
                value += GetTransitionProb(state, a, s) * (rewards[state, a, s] + gamma * valueFunction[s]);
            }
            actionValues[a] = value;
        }
        return actionValues;
    }

    public int GetOptimalAction(int state, float[] valueFunction)
    {
        float[] actionValues = GetActionValues(state, valueFunction);
        int optimalAction = 0;
        float optimalValue = actionValues[0];
        for (int a = 1; a < numActions; a++)
        {
            float value = actionValues[a];
            if (value > optimalValue)
            {
                optimalValue = value;
                optimalAction = a;
            }
        }
        return optimalAction;
    }

    public float[] ValueIteration(float threshold, int maxIterations)
     {
        Debug.Log("Hello");
         // Initialize the value function to zero
         float[] valueFunction = new float[numStates];

         // Set the threshold for convergence
         //float threshold = 1e-4f;

         // Set the discount factor
         float discountFactor = gamma;

         int numIterations = 0;

         // Perform the value iteration
         while (true)
         {
            numIterations++;
             // Initialize the maximum error to zero
             float maxError = 0.0f;

             for (int state = 0; state < numStates; state++)
             {
                 float oldValue = valueFunction[state];
                 float newValue = float.MinValue;

                 for (int action = 0; action < numActions; action++)
                 {
                     float qValue = 0.0f;

                     for (int nextState = 0; nextState < numStates; nextState++)
                     {
                         // Get the probability of transitioning to the next state from the current state-action pair
                         float prob = transitionProbs[state, action, nextState];

                         // Get the reward for transitioning to the next state from the current state-action pair
                         float reward = rewards[state, action, nextState];

                         // Get the value of the next state
                         float value = valueFunction[nextState];

                        // Use the Bellman equation to calculate the Q-value for the current state-action pair
                        qValue += prob * (reward + discountFactor * value);

                        Debug.Log($"State: {state}, Action: {action}, Q-value: {qValue}, Reward: {reward}");
                     }

                     newValue = Mathf.Max(newValue, qValue);
                    
                 }

                // Update the value of the current state to be the new value
                valueFunction[state] = newValue;
                // Update the maximum error to be the maximum difference between the old and new value of any state
                maxError = Mathf.Max(maxError, Mathf.Abs(oldValue - newValue));
             }

             //Debug.Log("Max error:" + maxError);

             if (maxError < threshold)
             {
                Debug.Log("Error Threshold Reached:  Iteration Number: " + numIterations);
                break;
             }
            // If the maximum error is below the specified threshold, break out of the loop and return the value function
            if (numIterations >= maxIterations)
             {
                Debug.Log("Max Iterations Reached:  Max Error: " + maxError);
                break;
            } 
         }

         return valueFunction;
     }

    public int[] GetOptimalPolicy(float[] valueFunction)
    {
        int[] policy = new int[numStates];
        for (int state = 0; state < numStates; state++)
        {
            float[] actionValues = new float[numActions];
            for (int action = 0; action < numActions; action++)
            {
                float value = 0;
                for (int nextState = 0; nextState < numStates; nextState++)
                {
                    float prob = transitionProbs[state, action, nextState];
                    float reward = rewards[state, action, nextState];
                    if (obstacleStates.Contains(nextState))
                    {
                        // Set the reward for obstacle states to be negative infinity
                        reward = float.NegativeInfinity;
                    }
                    value += prob * (reward + gamma * valueFunction[nextState]);
                }
                actionValues[action] = value;
            }
            policy[state] = Array.IndexOf(actionValues, actionValues.Max());
        }
        return policy;
    }
}
