# Unity Architecture Patterns

Explore the implementation of various software architecture patterns within the Unity Engine. This project demonstrates how the same game can be developed using different architectural approaches, providing practical insights into their strengths and trade-offs.

## Project Overview

Each architecture pattern is applied to create the same game, allowing for a direct comparison of coding styles, maintainability, scalability, and overall design efficiency. This approach offers a hands-on understanding of how different patterns affect game development in Unity.

![](https://github.com/SimonNordon4/unity-architecture-patterns/blob/main/resources/game_snapshot.gif)

The Game in Question, a Survivors Clone.

For a more detailed breakdown of the code structure, see the [Project Overview](resources/ProjectOverview.md).

## Architectural Patterns

### Spaghetti
The Spaghetti pattern is characterized by its lack of formal structure. Code is often written without clear organization, leading to tangled dependencies and difficulties in maintenance. This pattern is used as a baseline to highlight the advantages of more structured methodologies.

[Spaghetti Pattern Blog Post](https://medium.com/@simon.nordon/unity-architecture-spaghetti-pattern-7e995648c7c8/)

[Play the game made with the Spaghetti Pattern!](https://simonnordon4.github.io/unity-architecture-patterns/Builds/SpaghettiPattern/)

### GameObject-Component
This pattern is central to Unity's design philosophy, emphasizing modularity and reusability. Game objects are built using separate components, each responsible for specific functionalities. This approach enhances code readability, maintainability, and allows for easy iteration and testing of different game behaviors.

[GameObject Component Pattern Blog Post](https://medium.com/@simon.nordon/unity-architecture-gameobject-component-pattern-34a76a9eacfb/)

[Play the game made with the GameObject Component Pattern!](https://simonnordon4.github.io/unity-architecture-patterns/Builds/GameObjectComponentPattern/)

### Scriptable Object
This is Unity's new Paradigm pattern. It's desgined to work similarly to the GameObject Component Pattern, but leans of an extended use of Scriptable Objects to provide better Dependency Management compared to traditional methods.


[ScriptableObject Pattern Blog Post](https://medium.com/@simon.nordon/unity-architecture-gameobject-component-pattern-34a76a9eacfb/)

[Play the game made with the ScriptableObject Pattern!](https://medium.com/@simon.nordon/unity-architecture-scriptable-object-pattern-0a6c25b2d741)

### DOTS ECS
Planned

### Reactive Programming (R3)
Planned

### IoC Container + MVC (VContainer)
Planned

## Contributions

Contributions are welcome! Whether it's refining the existing patterns, adding new ones, or improving the documentation, your input can greatly benefit this project.

## Support

If you find this repository helpful, consider giving it a star! Your support motivates further development and helps others discover this resource.

## License

### Code

The C# code in this repository is released under the [MIT License](https://opensource.org/licenses/MIT). 

### Media Assets

All media assets (including but not limited to images, sound files, animations, shaders and 3D models) in this repository are proprietary. Unauthorized copying, modification, distribution, or use of these assets is strictly prohibited.

