using System.Collections.Generic;
using UnityEngine;

public class State
{
    public int id;               // unique identifier for the state
    public Vector2 position;     // position of the state on the grid
    public float reward;         // reward associated with the state
    public bool isTerminal;      // whether the state is a terminal state
    public List<MDP.Action> actions;  // list of possible actions from this state
    public Dictionary<MDP.Action, List<float>> transitionProbabilities;  // dictionary of transition probabilities for each action

    public State(int id, Vector2 position, float reward, bool isTerminal, List<MDP.Action> actions, Dictionary<MDP.Action, List<float>> transitionProbabilities)
    {
        this.id = id;
        this.position = position;
        this.reward = reward;
        this.isTerminal = isTerminal;
        this.actions = actions;
        this.transitionProbabilities = transitionProbabilities;
    }
}