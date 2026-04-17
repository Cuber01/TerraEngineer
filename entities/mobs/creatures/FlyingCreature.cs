using System;
using Godot;

namespace TerraEngineer.entities.mobs.creatures;

public abstract partial class FlyingCreature : Creature
{
    [Export] public float MarginAroundPoint = 20f;
    [Export] protected float TimeUntilBored = 5f;
    [Export] public Vector2 FlyAroundPoint;

    protected StateMachineWithTriggers<FlyingCreature, GenericCreatureTriggers> fsm;

    public Player Player;
    protected bool SeesPlayer = false;

    public class FlyingIdleState<T> : State<T> where T : FlyingCreature
    {
        private Vector2 goToPoint;
        private float delayAtPoint = 1f;

        public override void Enter()
        {
            void RerollPoint(ITimer timer)
            {
                if (!IsInstanceValid(Actor)) return;
                Vector2 rnd = MathT.RandomVector2(-Actor.MarginAroundPoint, Actor.MarginAroundPoint);
                goToPoint = Actor.FlyAroundPoint + rnd;
                TimerManager.Schedule(delayAtPoint, Actor, RerollPoint);
            }
            RerollPoint(null);
        }

        public override void Update(float dt)
        {
            Actor.CM.GetComponent<FreeFly>().FlyToPoint(goToPoint, dt);
        }
    }

    private void onDetectionAreaBodyEntered()
    {
        SeesPlayer = false;
        TimerManager.Schedule(TimeUntilBored, this, (_) =>
        {
            if (!SeesPlayer)
            {
                fsm.FireTrigger(GenericCreatureTriggers.EnemyLost);
            }
        });
    }

    private void onDetectionAreaBodyExited(Entity body)
    {
        SeesPlayer = true;
        fsm.FireTrigger(GenericCreatureTriggers.EnemyDetected);
    }
}
