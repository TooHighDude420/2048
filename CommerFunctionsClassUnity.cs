using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace commonFunctions
{
    public class CommerFunctionsClassUnity : MonoBehaviour
    {
        float timer;

        public void Timer(float delay, Action codeToRun)
        {
            timer += Time.deltaTime;

            if (timer >= delay)
            {
                codeToRun.Invoke();
                timer -= delay;
            }
        }


    }
}

