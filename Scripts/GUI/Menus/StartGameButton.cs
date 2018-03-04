using Godot;

class StartGameButton : Button
{
    public override void _Pressed()
    {
        GetTree().ChangeScene(Menu.GAMEPLAY_SCENE_PATH);
    }
}
