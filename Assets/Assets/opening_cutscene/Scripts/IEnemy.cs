using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    float health { get; }
    void takeDamage(float damage);

}
