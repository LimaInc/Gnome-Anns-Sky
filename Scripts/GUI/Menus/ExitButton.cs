using Godot;

class ExitButton : Button
{
    public override void _Pressed()
    {
        GetTree().Quit();
    }
}
