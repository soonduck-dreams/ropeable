using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MainUIOpenMode
{
    public enum OpenMode
    {
        Main,
        LevelSelect
    }

    public static OpenMode openMode = OpenMode.Main;
}
