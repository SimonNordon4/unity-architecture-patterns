In our last blog post, we delved into the 'Spaghetti' pattern, often the unintentional 
choice of new developers who are eager to bring their game ideas to life. 
While it allows for rapid development, it often results in unwieldy and complex code structures. 
Such codebases are notorious for their long, tangled methods, over reliance on 'God Objects' like 
Singleton Game Managers, and an endless cascade of bugs that seems to multiply with each new feature added.

This series aims to explore robust alternatives to the Spaghetti pattern. 

In this post, we turn our attention to the GameObject-Component Pattern,
originally envisioned as the ideal approach for Unity development. 

This pattern promises a solution to the chaos of spaghetti code by breaking down monolithic structures
into smaller, modular components. These components can be easily added, modified, or removed from GameObjects,
paving the way for a codebase that's both flexible and maintainable.

A classic 'Player Controller' under this system would be a 
GameObject composed of distinct 'Health', 'Movement', 'Input', and other such components.

Despite its theoretical benefits, 
the GameObject-Component Pattern isn't as widely used as one might expect, 
with Unity itself now favoring Scriptable Objects and ECS. To understand why, 
and to uncover both the strengths and weaknesses of this pattern, 

I dedicated over 50 hours to refactoring our Spaghetti Pattern Prototype, 
fully embracing the GameObject-Component approach in every aspect of the game's design.