using Godot;
using System;
using System.Collections.Generic;

public class Sandstorm : Spatial
{	
	private Vector2 direction;
	private Player player;
	private float lifeTime;
	private float timeAlive;

    public override void _Ready()
    {
		Random rnd = new Random();
		double rndRad = rnd.NextDouble() * 2.0 * Math.PI;
        direction = new Vector2(
			(float) Math.Cos(rndRad),
			(float) Math.Sin(rndRad)
			);
			
		player = GetNode(Game.PLAYER_PATH) as Player; 
		
		lifeTime = 30;      
		timeAlive = 0; 
    }

    public override void _Process(float delta)
    {
    	Console.WriteLine(this.GetPath());

		timeAlive += delta;
		if (lifeTime < timeAlive) GetParent().RemoveChild(this);
    }
	
	public override void _PhysicsProcess(float delta) {
		//So we need to check if the player is in cover
		//We do this by casting a ray up and in the direction of the wind
		
		Vector3 checkDir = 10 * new Vector3(
			-direction.x,
			2,
			-direction.y
		).Normalized();
		
		PhysicsDirectSpaceState spaceState = GetWorld()
			.GetDirectSpaceState();
		
		Vector3 zeroVec = new Vector3(0,0,0);
		Dictionary<object,object> collisions = spaceState.IntersectRay(
			player.ToGlobal(zeroVec),
			player.ToGlobal(checkDir),
			new Godot.Object[] {player}
		);
								
		if (collisions.Count == 0) {
			//So we're going to push.
			
			//to create a feling of being "hurled" we'll sin2 the current time.			
			float playerYpos = player.ToGlobal(zeroVec).y;
			
			float upThrust = 0;
			
			//50 is the highest the storm will carry the player upward
			if (playerYpos < 50)			
				upThrust = (float) (1 - Math.Cos(timeAlive / lifeTime * 5.0 * Math.PI));
						
			Vector3 pushAmount = new Vector3(
				direction.x,
				upThrust,
				direction.y
			).Normalized();	
						
			player.Push(pushAmount);
		}

	}
}
