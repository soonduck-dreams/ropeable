using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTraitManager : MonoBehaviour
{
    [SerializeField] private SettingsManager settingsManager;

    public UserTraitData userTraitData { get; private set; }

    private void Awake()
    {
        SaveLoadManager.instance.RequestToLoadUserTraitData();
    }

    public void InitUserTraitData()
    {
        userTraitData = new UserTraitData(true);
        SaveLoadManager.instance.RequestToSaveUserTraitData(userTraitData);
        settingsManager.SetCanChangeName(userTraitData.canChangeName);
    }

    public void SetUserTraitData(UserTraitData userTraitData)
    {
        this.userTraitData = userTraitData;
        settingsManager.SetCanChangeName(this.userTraitData.canChangeName);
    }
}