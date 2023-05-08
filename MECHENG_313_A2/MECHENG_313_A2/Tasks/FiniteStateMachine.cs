using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MECHENG_313_A2.Tasks
{
    public class FiniteStateMachine : IFiniteStateMachine
    {
        public void AddAction(string state, string eventTrigger, TimestampedAction action)
        {
            // TODO: Implement this
        }

        public string GetCurrentState()
        {
            // TODO: Implement this
            return null;
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
