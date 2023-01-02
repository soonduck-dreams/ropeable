using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextUtility : MonoBehaviour
{
    public static void DisplayAscendingText(string message, GameObject prefab, Vector3 position, Color color, float ascendSpeed, float lifeSeconds)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        TMP_Text text = instance.GetComponent<TMP_Text>();
        Ascender ascender = instance.GetComponent<Ascender>();

        text.text = message;
        text.color = color;
        ascender.ascendSpeed = ascendSpeed;
        ascender.lifeSeconds = lifeSeconds;
    }
}
