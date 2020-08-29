using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;
    [SerializeField] private Vector3 Offset;
    [SerializeField] private float YAxisThreshold;
    void Start() => player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

    private void Reset()
    {
        Offset = new Vector3(0, 0, -10);
        YAxisThreshold = 5;
    }
    
    private void Update()
    {
        float x = player.position.x + Offset.x;
        float y = player.position.y + Offset.y;
        y = Mathf.Clamp(transform.position.y, y - YAxisThreshold, y + YAxisThreshold);
        float z = player.position.z + Offset.z;
        transform.position = new Vector3(x,y,z);
    }
}
