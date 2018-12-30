using UnityEngine;
using System;
using System.Collections.Generic;

public interface IClassicHandlable
{
    IExecutable OnPlay
    {
        get;
        set;
    }
    IExecutable OnStop
    {
        get;
        set;
    }
    IExecutable OnUpdate
    {
        get;
        set;
    }
    IExecutable OnComplete
    {
        get;
        set;
    }
    void CopyFrom( IClassicHandlable obj );
}