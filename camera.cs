using Godot;
using System;

public class camera : Camera
{
    float camSpeed = 0.06f;
    float camRotateSpeed = 0.01f;
    public override void _Ready()
    {
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion)
        {
            Vector3 rot = this.GetRotation();

            InputEventMouseMotion emm = (InputEventMouseMotion) e;

            Vector2 rel = emm.GetRelative();

            int relX = (int) -rel.y;
            int relY = (int) -rel.x;

            Vector3 rotd = new Vector3(relX * camRotateSpeed, relY * camRotateSpeed, 0.0f);

            this.SetRotation(this.GetRotation() + rotd);
        }
    }

    public override void _Process(float delta)
    {   
        Vector3 rot = this.GetRotation();

        float sin = (float) (camSpeed * Math.Sin(rot.y));
        float cos = (float) (camSpeed * Math.Cos(rot.y));

        if (Input.IsActionPressed("forward"))
        {
            this.Translate(new Vector3(0.0f,0.0f,-camSpeed));
        }
        if (Input.IsActionPressed("backward"))
        {
            this.Translate(new Vector3(0.0f,0.0f,camSpeed));
        }
        if (Input.IsActionPressed("left"))
        {
            this.Translate(new Vector3(-camSpeed,0.0f,0.0f));
        }
        if (Input.IsActionPressed("right"))
        {
            this.Translate(new Vector3(camSpeed,0.0f,0.0f));
        }
    }
}
