using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TargetTracker : MonoBehaviour
{
    ObserverBehaviour imageTargetBehaviour;

    void Start()
    {
        imageTargetBehaviour = this.GetComponentInParent<ObserverBehaviour>();
        imageTargetBehaviour.OnTargetStatusChanged += onTargetStatusChanged;
    }

    void onTargetStatusChanged(ObserverBehaviour observerbehavour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED && status.StatusInfo == StatusInfo.NORMAL)
        {
            Debug.Log("Get Target Image!");
            this.gameObject.SetActive(true);
        }

        if (status.Status == Status.NO_POSE && status.StatusInfo == StatusInfo.NOT_OBSERVED)
        {
            Debug.Log("Miss Target Image!");
            this.gameObject.SetActive(false);
        }
    }
}