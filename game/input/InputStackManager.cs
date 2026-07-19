using Godot;
using System.Collections.Generic;
using System.Diagnostics;

namespace TerraEngineer.game.ui;


public partial class InputStackManager : Node
{
    private static Stack<InputContext> stack = new();

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        foreach (InputContext context in stack)
        {
            // if (@event.IsEcho())
            // {
            //     context.HandleEchoInput(@event);
            // }
            // else
            // {
            //     context.HandleNewInput(@event);
            // }
            
            bool consumed = context.HandleNewInput(@event);
            if (consumed)
            {
                #if INPUT_DEBUG
                GD.Print(@event.AsText() + " consumed.");
                #endif
                break;
            }
                
        }
        GetViewport().SetInputAsHandled();
    }
    
    public static void Push(InputContext context)
    {
        stack.Push(context);
    }
    
    public static void Pop()
    {
        stack.Pop();
    }
}