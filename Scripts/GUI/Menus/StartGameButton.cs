using Godot;

class StartGameButton : Button
{
    public override void _Pressed()
    {
        GetTree().ChangeScene(Menu.LOADING_SCENE_PATH);

        LoadingScreen ls = GetNode(Menu.LOADING_SCREEN_PATH) as LoadingScreen;
        ls.SetProcess(true);
    }
}
