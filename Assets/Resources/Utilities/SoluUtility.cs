using System;
using UnityEngine;

namespace SoluUtilities
{
    public static class SoluUtility
    {
        public static void SetActiveDisplay(GameObject go, bool isDisplay)
        {
            go.SetActive(isDisplay);
        }
    }
}