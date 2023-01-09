using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpeaker
{
    public void AddListener(IListener listener);
    public void RemoveListener(IListener listener);
    public void SpeakToListeners();
}
