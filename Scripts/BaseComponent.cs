using Godot;
using System;

public class BaseComponent : Node
{
    // Sets up a Connection if one does not already exist. Should be used in children classes in a
    // Process method to ensure custom connections are set up *after* relevant AddUserSignal has been called.
    protected void SetupConnection(string eventName, Node parent, Node obj, string methodName)
    {
        if(!parent.IsConnected(eventName, this, methodName))
        {
            parent.Connect(eventName, obj, methodName);
        }
    }
}
