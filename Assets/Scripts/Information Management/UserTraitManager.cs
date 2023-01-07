using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTraitManager : MonoBehaviour
{
    public UserTraitData userTraitData { get; private set; }

    private void Awake()
    {
        SaveLoadManager.instance.RequestToLoadUserTraitData();
    }

    public void InitUserTraitData()
    {
        userTraitData = new UserTraitData(true);
        SaveLoadManager.instance.RequestToSaveUserTraitData(userTraitData);
    }

    public void SetUserTraitData(UserTraitData userTraitData)
    {
        this.userTraitData = userTraitData;
    }
}
