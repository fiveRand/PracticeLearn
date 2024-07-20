using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Hat
}

public class PlayerStats : MonoBehaviour, IDamageable
{
    [SerializeField]
    int healthField;
    public int health 
    {
        get{ return healthField; }
        set
        {
            healthField = value;

            if(statUI != null)
            {
                statUI.DisplayHP(health, maxHealth);
            }
            if(value < 0)
            {
                Death();
            }
        }
    }
    [SerializeField]
    int defenseField;
    public int defense 
    {
        get{ return defenseField; }
        set
        {
            defenseField = value;
        }
    }
    [SerializeField]
    int maxHealthField;
    
    public int maxHealth 
    {
        get{ return maxHealthField; }
        set
        {
            maxHealthField = value;
        }
    }

    public SpriteRenderer hatSprite;
    public StatUI statUI;

    private void Start() {
        health = maxHealth;
    }
    public void Damage(int amount)
    {
        health -= amount;
    }

    public void Death()
    {
        Debug.Log("Player is Dead");
        Destroy(this.gameObject);
    }

    public void RestoreHealth(int amount)
    {
        health += amount;
    }
}
