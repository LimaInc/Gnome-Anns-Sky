using System;
using System.Diagnostics;
using System.Reflection;
using Godot;

class Debug
{
    private static int n = 0;

    public static string Message { get; set; }

    public static void PrintStackTrace(string id = null)
    {
        StackTrace st = new StackTrace();
        StackFrame[] frames = st.GetFrames();
        for (int i = 1; i < frames.Length; ++i)
        {
            StackFrame sf = frames[i];
            MethodBase mb = sf.GetMethod();
            string msg = (id != null ? id + ", in " : "In ") + mb.Name + " of " + mb.ReflectedType;
            if (sf.GetFileLineNumber() != 0)
            {
                msg += ", line " + sf.GetFileLineNumber();
            }
            if (sf.GetILOffset() == StackFrame.OFFSET_UNKNOWN)
            {
                msg += ", approximate offset " + sf.GetILOffset();
            }
            GD.Print(msg + "("+i+"/"+(frames.Length - 1)+")");
        }
    }

    public static void PrintPlace(string id = null, int stackFrameNum = 1)
    {
        StackFrame sf = new StackTrace().GetFrame(stackFrameNum);
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

    public static void PrintPlaceOccasionally(string str = null, int stackFrameNum = 2)
    {
        if (n % 100 == 0)
        {
            PrintPlace(str, stackFrameNum);
        }
    }

    public static void PrintPlaceOccasionallyEnd(string str = null, int stackFrameNum = 3)
    {
        PrintPlaceOccasionally(str, stackFrameNum);
        n++;
    }
}
