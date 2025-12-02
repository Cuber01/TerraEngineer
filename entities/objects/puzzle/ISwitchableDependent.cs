using Godot;

namespace TerraEngineer.entities.objects.puzzle;

// Type of SwitchableGroup members
public interface ISwitchableDependent
{
    public void OnSwitch(bool switchedOn);
}