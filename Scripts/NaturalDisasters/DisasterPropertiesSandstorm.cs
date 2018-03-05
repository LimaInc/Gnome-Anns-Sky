using Godot;
using System;

public class DisasterPropertiesSandstorm : DisasterProperties
{
	
	public DisasterPropertiesSandstorm() : base() {
		WarningTime = 10;
		Likelihood = 0.000;
	}

	public override Node StartEvent() {
		Node s = new Sandstorm();
		return s;
	}


}
