using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MECHENG_313_A2.Tasks
{
    
    public class FiniteStateMachine : IFiniteStateMachine
    {
        private string currentState;
        private Dictionary<string, Dictionary<string, StateInformation>> fst = new Dictionary<string, Dictionary<string, StateInformation>>();

        public struct StateInformation // Define a struct to represent our state/events
        {
            public string next;
            public List<TimestampedAction> actions;

            public StateInformation(string nextState, List<TimestampedAction> actionsList)
            {
                next = nextState;
                actions = actionsList;
            }
        }

        public void AddAction(string state, string eventTrigger, TimestampedAction action) // Implement 
        {
            if (!fst.ContainsKey(state)) // Check if the FST has the state
            {
                fst.Add(state, new Dictionary<string, StateInformation>()); // If it doesn't, add the row
            }

            if (!fst[state].ContainsKey(eventTrigger)) // Check if the FST doesn't yet have the event within the state, add the column. If the FST previously had no state, this will necessarily be true.
            {
                fst[state].Add(eventTrigger, new StateInformation(state, new List<TimestampedAction>())); // If it doesn't, create a new state/event at the state
            }

            fst[state][eventTrigger].actions.Add(action);
        }

        public string GetCurrentState()
        {
            return currentState;
        }

        public string ProcessEvent(string eventTrigger)
        {
            // TODO: Implement this
            return null;
        }

        public void SetCurrentState(string state)
        {
            // TODO: Implement this
        }

        public void SetNextState(string state, string nextState, string eventTrigger)
        {
            // TODO: Implement this
        }
    }
}
