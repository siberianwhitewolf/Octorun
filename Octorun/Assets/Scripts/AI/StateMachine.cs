using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public class StateMachine
    {
        private Dictionary<Enum, IState> states;
        private IState currentState;
        public StateMachine(Dictionary<Enum, IState> states, Enum startingState)
        {
            this.states = states;
            currentState = states[startingState];
            currentState.Enter();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void ChangeState(Enum newState)
        {
            currentState?.Exit();
            currentState = states[newState];
            currentState?.Enter();
        }
        
        public IState CurrentState => currentState;
    }
}
