using System.Collections;
using System.Collections.Generic;
using GameObjectComponent.App;
using UnityEngine;

public class NotificationTeseter : MonoBehaviour
{
    private NotificationManager _notificationScheduler;
    
    void Start()
    {
        _notificationScheduler = GetComponent<NotificationManager>();
    }

    [ContextMenu("Test")]
    public void Test()
    {
        _notificationScheduler.ScheduleNotification("Test");
    }
    
    
}
