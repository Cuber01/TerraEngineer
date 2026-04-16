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
        private const int StartingOffset = -6;
        private readonly Vector2 bigBulletStartingVelocity = new Vector2(0, -120);

        private const int R = 10;
        private const int NumbOfBullets = 8;
        
        private GravityBullet bigBullet;

        
        public override void Enter()
        {
            bigBullet = Actor.CM.GetComponent<GravityBulletSpawner>()
                .Start()
                .SetPosition(new Vector2(0, StartingOffset))
                .SetVelocity(bigBulletStartingVelocity)
                .SetOnLifetimeDeath(spawnRing)
                .Build();
            Actor.CM.GetComponent<GravityBulletSpawner>().AddToGame(Actor);
            
            Actor.fsm.FireTrigger(ToxicPlantTriggers.Shot);
        }



        private void spawnRing()
        {
            float angleBetweenPoints = (2*MathT.PI)/NumbOfBullets;
            for (int i = 0; i < NumbOfBullets; i++)
            {
                Vector2 targetPosition = new Vector2(bigBullet.GlobalPosition.X + R * Mathf.Sin(i*angleBetweenPoints),
                                                     bigBullet.GlobalPosition.Y + R * Mathf.Cos(i*angleBetweenPoints));
                Vector2 direction = (targetPosition - bigBullet.GlobalPosition).Normalized();
                bigBullet.CM.GetComponent<GravityBulletSpawner>()
                    .Start()
                    .SetPosition(bigBullet.Position)
                    .SetVelocity(direction * R * 5)
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
