# Lab 3 - Steering & Simple Swarm
### In your own words, what is the difference between a global path / waypoint list, and a local steering behaviour like Seek or Arrive?
A global path/waypoint list is usually calculated as a finished result through an algoritm. Local steering behaviour is more dynamic: it updates on each frame and reacts to it's environment/other objects. 

### What visual difference did you notice between an agent using Seek, and an agent using Arrive?
A seek will never end it's movement, versus the Arrive that will stop updating if it has reached it's target. Sadly, my Arrive acted up. It did something in between these two: it reached it's target and started oscilating, but the oscillation slowed down and eventually it stopped. 

### How did Separation change the behaviour of your group? What happens if you set separationStrength very low? Very high?
It created almost like a grid based layout among the agents. On very low, the clump together and eventually act as "one" Agent (stacked), and on very high they formed a very spacious grid. This is be due to that I also implemented Cohesion. 

### Looking ahead to your final project:
_Name at least one NPC, enemy, or unit that could use this SteeringAgent._
I think this mechanism/behaviour can be applied to my Guard unit in the stealth game I'm planning. 
_How might you combine steering with your FSM or pathfinding in that project?_
I'm thinking that this more "dynamic" steering behaviour can be switched on upon discovering the player, for example. When the player manages to hide again, the agent can be switched back to a more rigid (maybe FSM) behaviour. 
