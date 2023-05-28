using MECHENG_313_A2.Views;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace MECHENG_313_A2.Tasks
{

    public class FiniteStateMachine : IFiniteStateMachine
    {
        private string currentState;
        private string tempNextState;

        private Dictionary<string, Dictionary<string, StateInformation>> fst = new Dictionary<string, Dictionary<string, StateInformation>>();

        public struct StateInformation                      // Define a struct to represent our state/events
        {
            public string next;
            public List<TimestampedAction> actions;

            public StateInformation(string nextState, List<TimestampedAction> actionsList)
            {
                next = nextState;
                actions = actionsList;
            }
        }

        public void AddAction(string state, string eventTrigger, TimestampedAction action)
        {
            ExtendTable(state, eventTrigger);

            fst[state][eventTrigger].actions.Add(action);   // Add the action to the list of actions associated with the action/event
        }

        public string GetCurrentState()
        {
            return currentState;
        }

        public string GetNextState()
        {
            return tempNextState;
        }

        public string ProcessEvent(string eventTrigger)
        {
            // ProcessEvent takes an eventTrigger, and invokes the actions associated with the event, given the current state 
            // Note: There are two types of eventTrigger: tickEvent, and configEvent

            // Debug event
            //System.Diagnostics.Debug.WriteLine($"Processing event: {eventTrigger}");
            StateInformation currentStateInformation = fst[currentState][eventTrigger];
            tempNextState = currentStateInformation.next;
 
            List<TimestampedAction> actions = currentStateInformation.actions; // Figure out the actions, by accessing the FST

            // Debug new state
            //System.Diagnostics.Debug.WriteLine($"New state: {newState}");

            foreach (TimestampedAction action in actions)                        // For each of the actions,
            {
                ThreadPool.QueueUserWorkItem(state => action(DateTime.Now));    // Queue the action for the next available thread, using a lambda.
                                                                                // Note that variable state is needed but is not used.
            }

            return tempNextState; // Return the next state.// Usually, we will immediately invoke SetCurrentState to set currentState to nextState
        }


        public void SetCurrentState(string state)
        {
            currentState = state;

        }

        public void SetNextState(string state, string nextState, string eventTrigger)
        {
            ExtendTable(state, eventTrigger);

            StateInformation updatedStateInfo = fst[state][eventTrigger]; // Need to use a temp struct, otherwise we will be working on a copy instead of the actual variable
            updatedStateInfo.next = nextState;
            fst[state][eventTrigger] = updatedStateInfo; // Set the next state string associated with the action/event
        }

        public void ExtendTable(string state, string eventTrigger)
        {
            if (!fst.ContainsKey(state))                                    // Check whether the FST has the state
            {
                fst.Add(state, new Dictionary<string, StateInformation>()); // If it doesn't, add the row dictionary
            }

            if (!fst[state].ContainsKey(eventTrigger))              // Check whether the state dictionary has the event
            {                                                       // (If the FST previously had no state, this will necessarily be true.)
                fst[state].Add(eventTrigger, GetDefaultElement());  // If it doesn't, create it
            }
        }

        private StateInformation GetDefaultElement()
        {
            return new StateInformation("", new List<TimestampedAction>());
        }
    }
}