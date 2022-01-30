using System.Collections.Generic;
using Godot;
using System;

public abstract class Updater<T>
{

    public abstract void Update(T actor, int ticksCount);
    
    public class PlayerMove : Updater<Player>
    {

        public override void Update(Player player, int ticksCount)
        {
            var direction = Vector3.Zero;
            // We check for each move input and update the direction accordingly.
            if (Input.IsActionPressed("move_right"))
            {
                direction.x += 1;
            }
            if (Input.IsActionPressed("move_left"))
            {
                direction.x -= 1;
            }
            if (Input.IsActionPressed("move_down"))
            {
                direction.z += 1;
            }
            if (Input.IsActionPressed("move_up"))
            {
                direction.z -= 1;
            }
            
            player.translate = direction * player.speed;
            // velocity.y = vertical_velocity;
        }
    }

    public class PlayerDash : Updater<Player>
    {
        public override void Update(Player player, int ticksCount)
        {
            var direction = Vector3.Zero;
            // We check for each move input and update the direction accordingly.
            if (Input.IsActionPressed("move_right"))
            {
                direction.x += 1;
            }
            if (Input.IsActionPressed("move_left"))
            {
                direction.x -= 1;
            }
            if (Input.IsActionPressed("move_down"))
            {
                direction.z += 1;
            }
            if (Input.IsActionPressed("move_up"))
            {
                direction.z -= 1;
            }

            player.translate = direction.Normalized() * player.dashSpeed;
        }
    }
    public class Move : Updater<Player>
    {
        public int speedX;
        public int speedZ;

        public Move(int speedX, int speedZ)
        {
            this.speedX = speedX;
            this.speedZ = speedZ;
        }
        public override void Update(Player actor, int ticksCount)
        {
            actor.translate = Vector3.Zero;
        }
    }

    public class Attack : Updater<Player>
    {
        public int attackFrame;
        public int attackDamage;
        public AttackCollisionBox collisionBox;
        public Attack()
        {
        }

        public Attack(int attackFrame, int attackDamage, AttackCollisionBox collisionBox)
        {
            this.attackFrame = attackFrame;
            this.attackDamage = attackDamage;
            this.collisionBox = collisionBox;
        }

        public override void Update(Player actor, int ticksCount)
        {
            
        }
    }

    public class AutoReset : Updater<Player>
    {
        private readonly int ticks;

        public AutoReset(int ticks)
        {
            this.ticks = ticks;
        }
        public override void Update(Player actor, int ticksCount)
        {
            if (actor.countdownReset(ticks))
            {
                actor.submit(EventType.IDLE);
            }
        }
    }

    public class SelfDamage : Updater<Player>
    {
        public override void Update(Player actor, int ticksCount)
        {
            throw new NotImplementedException();
        }
    }

    public class Composite<T> : Updater<T>
    {
        private readonly Updater<T>[] updaters;

        public Composite(params Updater<T>[] updaters)
        {
            this.updaters = updaters;
        }
        public override void Update(T actor, int ticksCount)
        {
            foreach (var updater in updaters)
            {
                updater.Update(actor, ticksCount);
            }
        }
    }
        
}
