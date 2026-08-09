using Godot;

namespace TENamespace.basic.builders;

public interface ISpawner<T> where T : Node2D
{
    abstract T Build();
}