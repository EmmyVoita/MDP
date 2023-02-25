using System.Collections.Generic;
using UnityEngine;

public class MDP
{
    public enum Action
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
    }

    public int numStates;               // number of states in the MDP
    public int numActions;              // number of actions in the MDP
    public float discountFactor;        // discount factor for future rewards
    public List<State> states;          // list of states in the MDP

    public MDP(int numStates, int numActions, float discountFactor, List<State> states)
    {
        this.numStates = numStates;
        this.numActions = numActions;
        this.discountFactor = discountFactor;
        this.states = states;
    }

 

    public State NextState(State state, Action action)
    {
        // Compute the next state by sampling from the transition probabilities
        float p = Random.Range(0.0f, 1.0f);
        float cumulativeProbability = 0.0f;

        for (int nextStateId = 0; nextStateId < numStates; nextStateId++)
        {
            cumulativeProbability += state.transitionProbabilities[action][nextStateId];

            if (p <= cumulativeProbability)
            {
                return states[nextStateId];
            }
        }

        return states[numStates - 1];
    }
}
