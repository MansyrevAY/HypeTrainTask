using System;
using System.Collections.Generic;

public interface ITimerData
{
    void SetTimer(string name, DateTime dateTime);
    DateTime GetTimer(string name);
}
