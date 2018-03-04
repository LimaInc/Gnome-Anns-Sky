using Godot;

class StartGameButton : Button
{
    public override void _Pressed()
    {
        (GetNode(Menu.MAIN_MENU_PATH) as Control).Visible = false;

        LoadingScreen ls = GetNode(Menu.LOADING_SCREEN_PATH) as LoadingScreen;
        ls.Visible = true;
        ls.SetProcess(true);
    }
}
