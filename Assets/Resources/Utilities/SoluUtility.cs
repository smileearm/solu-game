using System;
using UnityEngine;

namespace SoluUtilities
{
    public static class SoluUtility
    {
        public static void SetActiveDisplay(GameObject gameObject, bool isDisplay)
        {
            gameObject.SetActive(isDisplay);
        }
    }
}