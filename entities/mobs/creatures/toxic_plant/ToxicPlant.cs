using Godot;
using System;
using TENamespace;
using TENamespace.basic.builders.gravity_bullet_spawner;
using TerraEngineer;
using TerraEngineer.entities.mobs.creatures;
using TerraEngineer.entities.projectiles.gravity_bullet;

public enum ToxicPlantTriggers
{
    Shot
}

public partial class ToxicPlant : Creature
{
    private readonly ShootState shootState = new ShootState();
    private readonly TimedState<ToxicPlant> waitState = new WaitState();

    private StateMachineWithTriggers<ToxicPlant,ToxicPlantTriggers> fsm;
    
    public override void Init()
    {
        fsm = new StateMachineWithTriggers<ToxicPlant,ToxicPlantTriggers>(this, waitState);
        fsm.AddTransition(shootState, waitState, (() => fsm.IsTriggered(ToxicPlantTriggers.Shot)));
        fsm.AddTransition(waitState, shootState, waitState.TimerCondition);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        #if TOOLS
        if (Engine.IsEditorHint())
            return;
        #endif
        
        fsm.Update((float)delta);
        CM.UpdateComponents((float)delta);
    }

    public class ShootState : State<ToxicPlant>
    {
        private const int startingOffset = 6;
        private readonly Vector2 bigBulletStartingVelocity = new Vector2(0, 50);

        private const int r = 10;
        private const int numbOfBullets = 8;
        
        private GravityBullet bigBullet;

        
        public override void Enter()
        {
            bigBullet = Actor.CM.GetComponent<GravityBulletSpawner>()
                .Start()
                .SetPosition(Actor.GlobalPosition + new Vector2(0, startingOffset))
                .SetVelocity(bigBulletStartingVelocity)
                .SetOnLifetimeDeath(spawnRing)
                .Build();
            Actor.CM.GetComponent<GravityBulletSpawner>().AddToGame(Actor);
            
            Actor.fsm.FireTrigger(ToxicPlantTriggers.Shot);
        }

        private void spawnRing()
        {
            float angleBetweenPoints = (2*MathT.PI)/numbOfBullets;
            for (int i = 0; i < numbOfBullets; i++)
            {
                Vector2 targetPosition = new Vector2(bigBullet.GlobalPosition.X + r * Mathf.Sin(angleBetweenPoints),
                                                     bigBullet.GlobalPosition.Y + r * Mathf.Cos(angleBetweenPoints));
                Vector2 direction = (targetPosition - Actor.GlobalPosition).Normalized();
                bigBullet.CM.GetComponent<GravityBulletSpawner>()
                    .Start()
                    .SetPosition(Actor.GlobalPosition)
                    .SetVelocity(direction * r * 10)
                    .Build();
                bigBullet.CM.GetComponent<GravityBulletSpawner>().AddToGame(Actor);
            }
        }
    }
    
    public class WaitState : TimedState<ToxicPlant>
    {
        public override void Enter()
        {
            base.Enter();
            Delay = 2.0f;
        }
    }

}
