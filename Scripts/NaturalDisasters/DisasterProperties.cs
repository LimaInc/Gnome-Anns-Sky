using Godot;
using System;

// this is a master class for describing the properties of a natural disaster
// that is, it's likelihood, warningTime (how long the warning runs for) and

abstract public class DisasterProperties
{	
	public double Likelihood;
	public int WarningTime;
	
	private float Counter;

    public DisasterProperties() {
		Counter = 0;
	}
	
	//call this to runthe counter. Returns true when the counter is full
    public bool Process(float delta) {
		Counter += delta;
		
		return (Counter > WarningTime);
    }

	//Call this to start the event
	//Returns a node that represents the event
    abstract public Node StartEvent();
}
