using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableItem
{
    void OnInteract(PlayerStats stats);
}
