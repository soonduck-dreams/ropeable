using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void SaveInteger(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public void SaveFloat(string key, int value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public void SaveArray(string key, Array arr)
    {
        StringBuilder value = new StringBuilder();
        int len = arr.Length;

        for(int i = 0; i < len; i++)
        {
            value.Append(arr.GetValue(i).ToString());

            if (i < len - 1)
            {
                value.Append(',');
            }
        }

        PlayerPrefs.SetString(key, value.ToString());
    }

    public bool LoadInteger(string key, out int value)
    {
        if (PlayerPrefs.HasKey(key))
        {
            value = PlayerPrefs.GetInt(key, -1);

            if (value < 0)
            {
                return false;
            }

            return true;
        }

        value = 0;
        return false;
    }

    public bool LoadFloat(string key, out float value)
    {
        if (PlayerPrefs.HasKey(key))
        {
            value = PlayerPrefs.GetFloat(key, -1);

            if (value < 0)
            {
                return false;
            }

            return true;
        }

        value = 0;
        return false;
    }

    public bool LoadArray(string key, out int[] arr)
    {
        string value;

        arr = null;

        if (PlayerPrefs.HasKey(key))
        {
            value = PlayerPrefs.GetString(key);

            if (value == "")
            {
                return false;
            }

            arr = StringToIntArray(value);

            return true;
        }

        return false;
    }

    public bool LoadArray(string key, out float[] arr)
    {
        string value;

        arr = null;

        if (PlayerPrefs.HasKey(key))
        {
            value = PlayerPrefs.GetString(key);

            if (value == "")
            {
                return false;
            }

            arr = StringToFloatArray(value);

            return true;
        }

        return false;
    }

    public bool LoadArray(string key, out bool[] arr)
    {
        string value;

        arr = null;

        if (PlayerPrefs.HasKey(key))
        {
            value = PlayerPrefs.GetString(key);

            if (value == "")
            {
                return false;
            }

            arr = StringToBoolArray(value);

            return true;
        }

        return false;
    }

    private int[] StringToIntArray(string str)
    {
        string[] stringArr = str.Split(',');
        int len = stringArr.Length;

        int[] result = new int[len];

        for (int i = 0; i < len; i++)
        {
            result[i] = Int32.Parse(stringArr[i]);
        }

        return result;
    }

    private float[] StringToFloatArray(string str)
    {
        string[] stringArr = str.Split(',');
        int len = stringArr.Length;

        float[] result = new float[len];

        for (int i = 0; i < len; i++)
        {
            result[i] = float.Parse(stringArr[i]);
        }

        return result;
    }

    private bool[] StringToBoolArray(string str)
    {
        string[] stringArr = str.Split(',');
        int len = stringArr.Length;

        bool[] result = new bool[len];

        for (int i = 0; i < len; i++)
        {
            result[i] = Convert.ToBoolean(stringArr[i]);
        }

        return result;
    }
}