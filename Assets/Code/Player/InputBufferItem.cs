using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBufferItem
{
    public int hold;
    public bool used;

    public bool CanExecute()
    {
        if (hold == 1 && !used) { return true; }
        return false;
    }

    public void Execute()
    {
        used = true;
    }

    public void Hold()
    {
        if (hold < 0)
        {
            hold = 1;
        }
        else
        {
            hold += 1;
        }
    }

    public void ReleaseHold()
    {
        if (hold > 0)
        {
            hold = -1;
            used = false;
        }
        else
        {
            hold = 0;
        }
    }

    public void ForceHold()
    {
        hold = 2;
    }

    public void Reset()
    {
        hold = 0;
        used = false;
    }
}
