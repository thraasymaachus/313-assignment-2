using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MECHENG_313_A2.Tasks
{
    
    public class FiniteStateMachine : IFiniteStateMachine
    {
        private string currentState;
        private Dictionary<string, Dictionary<string, StateInformation>> fsm = new Dictionary<string, Dictionary<string, StateInformation>>();

        public struct StateInformation
        {
            public string next;
            TimestampedAction[] actions;

            public StateInformation(string nextState, TimestampedAction[] actionsList)
            {
                next = nextState;
                actions = actionsList;
            }
        }

        public void AddAction(string state, string eventTrigger, TimestampedAction action)
        {
            // TODO: Implement this
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
