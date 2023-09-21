using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [ReadOnly]
    public OwnerTypes OwnerType;

    public Rigidbody Rigidbody;
}
