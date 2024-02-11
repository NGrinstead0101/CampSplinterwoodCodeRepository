/*****************************************************************************
// File Name :         Subject.cs
// Author :            Nick Grinstead
// Creation Date :     Oct 3rd, 2023
//
// Brief Description :  This interface represents a Subject that registers Observers
                        and pushes provides them with updated information.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Subject
{
    public abstract void RegisterObserver(Observer newObserver);

    public abstract void RemoveObserver(Observer observerToRemove);

    public abstract void UpdateObservers();
}
