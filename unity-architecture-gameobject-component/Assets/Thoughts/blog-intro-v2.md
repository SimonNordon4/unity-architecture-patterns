In the last blog post we looked into the 'Spaghetti' pattern, a joke word used to describe a game that doesn't use
any pattern at all. It's the easiest way to get started with Unity, and the pattern of choice for 
many new developers who are eager to bring their game ideas to life. Unfortunately many developers fail to expand
their knowledge beyond this point.

As we found previously, this 'spaghetti' pattern makes building
a game fast and easy. However it quickly devolves into a mess of code, resulting in unwieldy and complex code structures.

Such codebases are notorious for their long, tangled methods, over reliance on 'God Objects' like
Singleton Game Managers, and an endless cascade of bugs that seems to multiply with each new feature added.

Fortunately, there are alternative patterns out there. Particular to this writing, we'll be taking a look at the
GameObject-Component Pattern, which was Unity's original vision and official framework for fixing many of the issues
caused by spaghetti code.

It does this by breaking down those large 'God Objects' into smaller, modular components that can be easily added, modified or removed
from GameObjects without having to write any new code. This, in theory, leads to a more flexible and maintainable codebase,
as creating new behaviours is as easy as slapping a bunch of components together.

So, if this pattern is so great, why does no one seem to use it? Even Unity is leaving it behind, preferring Scriptable Objects and ECS instead.

I spent over 50 hours refactoring the Spaghetti Pattern Prototype to fully utilize GameObject Component in every facet, 
so that we can discover its strengths, but also its weaknesses.

