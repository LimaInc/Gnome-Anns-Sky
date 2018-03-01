using Godot;

public interface ITerrainModifier
{
    void UpdateHeight(Vector2 worldCoords, ref int height);
}
