using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="EventCenter")]
public class EventCenterSO : ScriptableObject
{
    public UnityAction<Vector2Int> click;
    public UnityAction<Vector2Int> flag;
    public UnityAction rollback;
    public UnityAction TimerStart;
}
