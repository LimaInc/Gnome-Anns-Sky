using Godot;

public class Label2D : Node2D
{
    private Label label;

    public Label2D() : base()
    {
        label = new Label();
        this.AddChild(label);
    }

    public string Text { get => label.Text; set => label.Text = value; }
}