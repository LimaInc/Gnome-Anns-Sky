using Godot;
using System;
using System.Collections.Generic;

public class DisasterManager : WorldEnvironment
{
    private List<DisasterProperties> allDisasters;
	private bool disasterHappening;
	private DisasterProperties currentDisaster;

	//this is probably very very wrong, but here's how this class works
	
	//I have a list of disaster classes. allDisasters.
	
	//The likelihood of each disaster is just the probability it happensi
    public override void _Ready()
    {
		disasterHappening = false;
        allDisasters = new List<DisasterProperties>();
		
		allDisasters.Add(new DisasterPropertiesSandstorm());
    }

    public override void _Process(float delta) {
        if (!disasterHappening) {
			//Each disaster has a likelihood of happening in a given second.
			
			//These disasters cannot happen simultaneously though. So instead
			//I iterate over the whole list.
			
			Random rnd = new Random();
			
			foreach (DisasterProperties disasterProperty in allDisasters) {
				double rndVal = rnd.NextDouble();
				Console.WriteLine(rndVal);
				Console.WriteLine(disasterProperty.Likelihood * delta);
				
				if (rndVal < disasterProperty.Likelihood * delta) {
					disasterHappening = true;
					currentDisaster = disasterProperty;	
				}
			}
		} else {
			//if a disaster is currently happening. Then run it until it gives you an object.
			if (currentDisaster.Process(delta)) {
				AddChild(currentDisaster.StartEvent());
				disasterHappening = false;
			}
		}
    }
}
