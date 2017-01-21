using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    enum StateMethod
    {
        Enter,
        Update,
        Exit
    }

    class FSM
    {
        public delegate void FSMFunction(StateMethod method, float deltaTime);

        private List<FSMFunction> states = new List<FSMFunction>();
        private int currentState = -1;

        public void AddState(FSMFunction function)
        {
            states.Add(function);
        }

        public void ChangeState(int state)
        {
            if (currentState != -1)
            {
                states[currentState].Invoke(StateMethod.Exit, 0f);
            }

            currentState = state;

            states[currentState].Invoke(StateMethod.Enter, 0f);
        }

        public void Update(float deltaTime)
        {
            if (currentState != -1)
            {
                states[currentState].Invoke(StateMethod.Update, deltaTime);
            }
        }
    }
}
