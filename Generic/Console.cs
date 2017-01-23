using System;
using UnityEngine;


public static class Console {

    public static void Log(params object[] parameters) {
        string formatter = "";
        for (int idx = 0; idx < parameters.Length; idx++) {
            formatter += "{" + idx.ToString() + "}, ";
        }
        Debug.Log(String.Format(formatter, parameters));
    }
}