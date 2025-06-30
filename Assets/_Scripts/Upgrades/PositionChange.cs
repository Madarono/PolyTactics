using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionChange : MonoBehaviour
{
    public Transform[] places;

    public void MovePlaces(int index)
    {
        transform.position = places[index].position;
    }
}
