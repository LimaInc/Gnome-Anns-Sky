using System;
using System.Diagnostics;
using System.Reflection;
using Godot;

class Debug
{
    public static string Message { get; set; }

    public static void PrintPlace(string id = null)
    {
        StackFrame sf = new StackTrace().GetFrame(1);
        MethodBase mb = sf.GetMethod();
        string msg = (id != null ? id+", in " : "In ") + mb.Name + " of " + mb.ReflectedType;
        if (sf.GetFileLineNumber() != 0)
        {
            msg += ", line " + sf.GetFileLineNumber();
        }
        if (sf.GetILOffset() == StackFrame.OFFSET_UNKNOWN)
        {
            msg += ", approximate offset " + sf.GetILOffset();
        }
        GD.Print(msg);
    }
}
