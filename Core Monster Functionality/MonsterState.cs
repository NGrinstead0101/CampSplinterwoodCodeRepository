/*****************************************************************************
// File Name :         MonsterState.cs
// Author :            Nick Grinstead
// Creation Date :     Sep 30th, 2023
//
// Brief Description :  This interface represents a state the monster can be in
                        along with its required methods. It also tracks the monster's
                        target destination.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MonsterState
{
    public static Vector3 destination;

    public abstract void UpdateDestination();

    public abstract bool CheckForUpdate();

    public void CheckForStateChange();

    public void ResetState();
}
