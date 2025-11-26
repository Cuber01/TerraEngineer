using System;

namespace TerraEngineer.entities.objects.puzzle;

public interface ISwitcher
{
    public delegate void SwitchedEventHandler(bool switchedOn);
    public event SwitchedEventHandler Switched;
    
    // TODO Remember to call event in implementation!
    bool SwitchedOn { get; set; }
}