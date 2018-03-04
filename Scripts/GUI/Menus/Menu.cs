using Godot;

// TODO: figure out Godot containers and replace anchors with them in the scene file
public class Menu : Node
{
    public const string MENU_SCENE_PATH = Game.SCENES_PATH + "/MenuScene.tscn";
    public const string LOADING_SCENE_PATH = Game.SCENES_PATH + "/LoadingScene.tscn";
    public const string GAMEPLAY_SCENE_PATH = Game.SCENES_PATH + "/GameplayScene.tscn";
    
    public const string MENUS_PATH = "/root/Menus";
    public const string MAIN_MENU_PATH = MENUS_PATH + "/MainMenu";
    public const string LOADING_SCREEN_PATH = "/root/LoadingScene";
    public const string LOADING_PROGRESS_PATH = LOADING_SCREEN_PATH + "/Loading/ProgressBar";

    public Menu()
    {
        LoadingScreen ls = GetNode(LOADING_SCREEN_PATH) as LoadingScreen;
        ls.Visible = false;
    }
}
