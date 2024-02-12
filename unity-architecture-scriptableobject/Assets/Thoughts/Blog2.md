Unity Architecture: Scriptable Object Pattern.

What is the Scriptable Object Pattern?

(i.e. using ScriptableObjects for runtime data and events)

How is it different to the Spaghetti and GameObject Component Pattern?

What Advantages does it have over those two?

Examples of implementation. (show from game)

12/02/2024.

We're so used to using MonoBehaviours, we seldom stop to ask ourselves, why are we using them?

Let's list all the things that we would need to use a MonoBehaviour for:

- Calling Update(), FixedUpdate() and LateUpdate()
- Calling collision events like OnCollisionEnter(), OnTriggerEnter(), etc.
- We need a concrete reference to another MonoBehaviour or GameObject in the scene.
- An unknown quantity will be instantiated during runtime.

It turns out, that's it. Which means if our current behaviours aren't relying on these things,
They can be scriptable objects.

So all our objects that would have been Singletons, like Game Manager, Achievement Manager, Settings etc
can now be project wide Scriptable Objects.

Our singular runtime data, like that player's health, score, etc can also be Scriptable Objects.

Our event containers, like OnPlayerDeath, OnPlayerRespawn, etc can also be Scriptable Objects.

Even our functional data, like object pools and chest spawner, can be ScriptableObjects.
