using Godot;

class LoadingScreen : Control
{
    private const int maxTime = 1000 / 60;

    private ResourceInteractiveLoader loader;

    public override void _Ready()
    {
        loader = ResourceLoader.LoadInteractive(Menu.GAMEPLAY_SCENE_PATH);
        GD.Print("Dependencies: ");
        foreach(string dependency in ResourceLoader.GetDependencies(Menu.GAMEPLAY_SCENE_PATH))
        {
            GD.Print(dependency);
        }
        SetProcess(false);
    }

    public override void _Process(float delta)
    {
        if (loader != null)
        {
            float timeUp = OS.GetTicksMsec() + maxTime;
            Error err = Error.Ok;
            while (err != Error.FileEof && OS.GetTicksMsec() <= timeUp)
            {
                err = loader.Poll();
                GD.Print("Progress: " + loader.GetStage() / (float)loader.GetStageCount());
                switch (err)
                {
                    case Error.FileEof:
                        PackedScene scene = loader.GetResource() as PackedScene;
                        GetTree().ChangeSceneTo(scene);
                        break;
                    case Error.Ok:
                        // TODO: implement progress bar, then update it here
                        break;
                    default:
                        GD.Print(err);
                        GetTree().Quit();
                        break;
                }
            }
        }
    }
}
