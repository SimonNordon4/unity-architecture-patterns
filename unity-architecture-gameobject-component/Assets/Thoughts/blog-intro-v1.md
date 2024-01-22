This a continuation of the Unity Architecture Series. In the last post, we created a game in 50 hours using the Spaghetti Pattern.

In this post, we’re going to explore the GameObject Component Architecture,
which is Unity’s original vision and official framework for making games.

The code for this project is also available on GitHub if you’d like to do a deep dive.


First, what is the GameObject Component Pattern?

GameObject-Component Architecture breaks down complex game functionalities into modular, 
interchangeable components, significantly enhancing code maintenance and flexibility.


Player Controller from Spaghetti Pattern (left) versus new Player from Game Object Component (right)
Its core strength lies in the ability to easily add, modify, or remove behaviors from game objects
without needing to overhaul their basic structure.

An example would be decomposing our Player Controller into several agnostic MonoBehaviours like Health, Movement, Dash, 
Melee Attack etc with each following the Single Responsibility Principle.

However, if this pattern is so great, why does no one seem to use it? Even Unity is leaving it behind, preferring
Scriptable Objects and ECS instead.

I’ve spent over 50 hours refactoring the Spaghetti Pattern Prototype to fully utilize GameObject Component in order
to discover its strengths, and its weaknesses.