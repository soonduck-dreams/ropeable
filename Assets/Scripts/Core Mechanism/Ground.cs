using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour, IRopeable
{
    public string GetTerrainType()
    {
        return "ground";
    }

    public void OnRopeHang()
    {

    }

    public void OnRopeCut()
    {

    }
}
