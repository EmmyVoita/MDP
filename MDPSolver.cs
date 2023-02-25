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
        float delta = 0;

        do
        {
            delta = 0f;

            foreach (State state in mdp.states)
            {
                if (!state.isTerminal)
                {
                    float v = state.reward;
                    state.reward = float.NegativeInfinity;

                    foreach (MDP.Action action in Enum.GetValues(typeof(MDP.Action)))
                    {
                        if (state.transitionProbabilities.ContainsKey(action))
                        {
                            float q = 0f;

                            foreach (State nextState in mdp.states)
                            {

                                //Debug.Log("id: " + nextState.id + "  position: " + nextState.position + " reward: " + nextState.reward);


                                if (state.transitionProbabilities.ContainsKey(action) && state.transitionProbabilities[action].Contains(nextState.id))
                                {
                                    Debug.Log($"transitionProbabilities for action {action} does contain key {nextState.id}");
                                    q += state.transitionProbabilities[action][nextState.id] * nextState.reward;
                                }
                                else
                                {
                                    Debug.Log($"transitionProbabilities for action {action} does not contain key {nextState.id}");
                                }

                            }

                            state.reward = Mathf.Max(state.reward, mdp.ExpectedReward(state, action) + mdp.discountFactor * q);
                        }
                    }

                    delta = Mathf.Max(delta, Mathf.Abs(v - state.reward));
                }
            }
        } while (delta > epsilon);
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

                    foreach (State nextState in mdp.states)
                    {
                        q += state.transitionProbabilities[action][nextState.id] * nextState.reward;
                    }

                    float value = mdp.ExpectedReward(state, action) + mdp.discountFactor * q;

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


