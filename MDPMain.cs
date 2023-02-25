using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDPMain : MonoBehaviour
{
    public int numStatesX = 4;
    public int numStatesY = 4;
    public int numActions = 4;
    public float discountFactor = 0.99f;


    void Start()
    {
        // Create states
        List<State> states = new List<State>();

        for (int x = 0; x < numStatesX; x++)
        {
            for (int y = 0; y < numStatesY; y++)
            {
                int id = x * numStatesY + y;
                Vector2 position = new Vector2(x, y);
                float reward = -1f;
                bool isTerminal = false;

                if (x == 0 && y == 0)
                {
                    isTerminal = true;
                    reward = 0f;
                }
                else if (x == 3 && y == 3)
                {
                    isTerminal = true;
                    reward = 0f;
                }

                List<MDP.Action> actions = new List<MDP.Action>();
                Dictionary<MDP.Action, List<float>> transitionProbabilities = new Dictionary<MDP.Action, List<float>>();

                if (!isTerminal)
                {
                    actions.Add(MDP.Action.MoveUp);
                    actions.Add(MDP.Action.MoveDown);
                    actions.Add(MDP.Action.MoveLeft);
                    actions.Add(MDP.Action.MoveRight);

                    float p = 0.25f;

                    transitionProbabilities[MDP.Action.MoveUp] = new List<float>() { p, 0f, 0f, 0f };
                    transitionProbabilities[MDP.Action.MoveDown] = new List<float>() { 0f, p, 0f, 0f };
                    transitionProbabilities[MDP.Action.MoveLeft] = new List<float>() { 0f, 0f, p, 0f };
                    transitionProbabilities[MDP.Action.MoveRight] = new List<float>() { 0f, 0f, 0f, p };
                }

                State state = new State(id, position, reward, isTerminal, actions, transitionProbabilities);
                states.Add(state);
            }
        }

        // Create MDP
        int numStates = states.Count;
        MDP mdp = new MDP(numStates, numActions, discountFactor, states);

        // Solve MDP
        MDPSolver solver = new MDPSolver(mdp);
        float epsilon = 0.0001f;
        solver.ValueIteration(epsilon);
        solver.CalculatePolicyFromValues();

        // Print policy
        for (int i = 0; i < numStates; i++)
        {
            Debug.Log("State " + i + " policy: " + solver.GetPolicy(i));
        }
    }

}
