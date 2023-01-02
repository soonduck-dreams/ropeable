using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRopeable
{
    string GetTerrainType();
    void OnRopeHang();
    void OnRopeCut();
}
