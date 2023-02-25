using System;
using System.Collections.Generic;
using UnityEngine;

public class MDPSolver
{
    private MDP mdp;
    private float[] values;
    private int[] policy;

    public MDPSolver(MDP mdp)
    {
        this.mdp = mdp;
        this.values = new float[mdp.numStates];
        this.policy = new int[mdp.numStates];
    }


    public void ValueIteration(float epsilon)
    {
        // Initialize the values of all states to 0
        for (int i = 0; i < mdp.numStates; i++)
        {
            values[i] = 0.0f;
        }

        // Perform value iteration until convergence
        bool converged = false;
        while (!converged)
        {
            float delta = 0.0f;

            // Compute the new value of each state as the maximum expected reward over all actions
            for (int i = 0; i < mdp.numStates; i++)
            {
                State state = mdp.states[i];

                if (!state.isTerminal)
                {
                    float oldValue = values[i];
                    float newValue = float.NegativeInfinity;

                    foreach (MDP.Action action in state.actions)
                    {
                        float q = 0.0f;

                        // Compute the expected reward for taking the given action in the current state
                        for (int j = 0; j < mdp.numStates; j++)
                        {
                            float probability = state.transitionProbabilities[action][j];
                            q += probability * values[j];
                        }

                        // Compute the new value of the state as the maximum expected reward over all actions
                        float value = ExpectedReward(state, action) + mdp.discountFactor * q;
                        if (value > newValue)
                        {
                            newValue = value;
                        }
                    }

                    values[i] = newValue;
                    delta = Mathf.Max(delta, Mathf.Abs(newValue - oldValue));
                }
            }

            if (delta < epsilon)
            {
                converged = true;
            }
        }

        CalculatePolicyFromValues();
    }

    public float ExpectedReward(State state, MDP.Action action)
    {
        float expectedReward = 0.0f;

        // Compute the expected reward as the sum of the rewards weighted by the transition probabilities
        foreach (State nextState in mdp.states)
        {
            float transitionProb = state.transitionProbabilities[action][nextState.id];
            float reward = nextState.reward;
            expectedReward += transitionProb * (reward + mdp.discountFactor * values[nextState.id]);
        }

        return expectedReward;
    }

    public int GetPolicy(int stateId)
    {
        return policy[stateId];
    }

    public float GetValue(int stateId)
    {
        return values[stateId];
    }

    public void CalculatePolicyFromValues()
    {
        foreach (State state in mdp.states)
        {
            if (!state.isTerminal)
            {
                float maxValue = float.NegativeInfinity;
                MDP.Action bestAction = 0;

                foreach (MDP.Action action in Enum.GetValues(typeof(MDP.Action)))
                {
                    float q = 0f;

                    // Compute the expected value of the next state using the precomputed values array
                    for (int nextStateId = 0; nextStateId < mdp.numStates; nextStateId++)
                    {
                        q += state.transitionProbabilities[action][nextStateId] * values[nextStateId];
                    }

                    float value = q;

                    if (value > maxValue)
                    {
                        maxValue = value;
                        bestAction = action;
                    }
                }

                policy[state.id] = (int)bestAction;
            }
        }
    }
}


