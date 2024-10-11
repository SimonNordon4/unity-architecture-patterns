# Update

I have not updated this repo in a while, and it only contains 2 patterns at the moment. Big Update coming soon!

I've decided to refactor the original project and there are 2 primary reasons for this.

## Reason One: Not Complex Enough

Managing scenes are a huge hurdle in Unity. Unfortunately, this project takes place in a single scene! This fails to highlight the strengths of many architecture patterns that deal with intra-scene dependencies and asycnhronous changes. So the updated version will include both Single and Additive scene loading to address this.

## Reason Two: Too Complex
The game is very simply in some ways, and needlessly complex in others. The huge variety of content makes refactoring a much more labarious task than it needs to be. For that reason, the following features will be cut:

Reduced Features:
- The 'store' will be removed and it's upgrades moved to chests.
- Number of achievements will be reduced.
- Sword attack will be removed from the game.
- Enemy spawning will be greatly simplified. Instead it will just be a 30 minutes session of increasing enemy difficulty.
- You will no longer be able to interrupt enemy spawnings (This added way more complexity than it should have).
- Number of upgrades will be reduced.
- Wave Progress bar will be removed.

New Features:
  - New Main Menu Scene. To highlight single scene loading.
  - New Level. To highlight additive scene loading.
  - New difficult levels. Many people said the game was too hard.
  - Leaderboards. To highlight asynchronous service support.

# Unity Architecture Patterns

Explore the implementation of various software architecture patterns within the Unity Engine. This project demonstrates how the same game can be developed using different architectural approaches, providing practical insights into their strengths and trade-offs.

## Project Overview

Each architecture pattern is applied to create the same game, allowing for a direct comparison of coding styles, maintainability, scalability, and overall design efficiency. This approach offers a hands-on understanding of how different patterns affect game development in Unity.

![](https://github.com/SimonNordon4/unity-architecture-patterns/blob/main/resources/game_snapshot.gif)

The Game in Question, a Survivors Clone.


## Architectural Patterns

### Spaghetti
The Spaghetti pattern is characterized by its lack of formal structure. Code is often written without clear organization, leading to tangled dependencies and difficulties in maintenance. This pattern is used as a baseline to highlight the advantages of more structured methodologies.

### GameObject-Component
This pattern is central to Unity's design philosophy, emphasizing modularity and reusability. Game objects are built using separate components, each responsible for specific functionalities. This approach enhances code readability, maintainability, and allows for easy iteration and testing of different game behaviors.

### Scriptable Object
Coming Soon

### DOTS ECS
Planned

### Reactive Programming (UniRx)
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

