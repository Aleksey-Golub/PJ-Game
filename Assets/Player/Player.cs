using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Update()
    {
        Input.GetAxisRaw("Horizontal");
        Input.GetAxisRaw("Vertical");
    }
}
