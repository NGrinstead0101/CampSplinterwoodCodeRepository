/*****************************************************************************
// File Name :         Observer.cs
// Author :            Nick Grinstead
// Creation Date :     Oct 3rd, 2023
//
// Brief Description :  This interface represents an Observer that gets information
                        from a Subject.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Observer
{
    public abstract void ReceiveSubjectInfo(float modifier);
}
