using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int maxHealth{ get; set; }
    int health{ get; set; }
    int defense{ get; set; }
     void Death();
     void Damage(int amount);
     void RestoreHealth(int amount);
}
