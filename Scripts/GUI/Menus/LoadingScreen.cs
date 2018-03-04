using Godot;

class LoadingScreen : Control
{
    private const int maxTime = 1000 / 60;

    private ResourceInteractiveLoader loader;
    private ProgressBar progressBar;

    public override void _Ready()
    {
        loader = ResourceLoader.LoadInteractive(Menu.GAMEPLAY_SCENE_PATH);
        progressBar = GetNode(Menu.LOADING_PROGRESS_PATH) as ProgressBar;
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
                switch (err)
                {
                    case Error.FileEof:
                        PackedScene scene = loader.GetResource() as PackedScene;
                        GetTree().ChangeSceneTo(scene);
                        SetProcess(false);
                        break;
                    case Error.Ok:
                        progressBar.Value = loader.GetStage() / (float)loader.GetStageCount();
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
