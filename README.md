# space-invaders-webgl-app

How To Play:

Player can press S and D key to move the spaceship to left and right side, and press Spacebar or Left Mouse Button to shoot projectile to hit enemies, in order to win a game session.


Project Overview:

This game is made in 72 hours, and all game and UI logic are self-coded, which all self-created codes are located inside '0_Scripts' folder for review.

Since the game has to be completed in a short period, Singleton pattern is used to reduce the complexity of the development. For better decoupling or modularity, Dependency Injection framework such as Zenject or Extenject are recommended.

Though, Observer pattern is used with improved custom MessageDispatcher plugin to greatly improve decoupling, so the game can be scaled with minimal effort.

The codebase also demonstrated the uses of LINQ, UniRX, Exception Handling, etc, to showcase the understanding of advanced coding knowledge.

All enemies, projectiles, and explosion effects are pooled with a simple PoolManager.

Texture optimization is slightly performed to reduce build size. For better optimization, sprite atlas or asset bundle implementation are recommended.

Project Highlights:

1. Game is made in 72 hours
2. Game has 4 enemy types
3. Game UIs are considerably polished
4. Game has a leaderboard that shows player ranking, and the leaderboard data can be loaded upon game restart
5. All game logics are UIs are self-created
6. Singleton pattern is applied for quick prototyping
7. Observer pattern is applied for better decoupling and modularity
8. All enemies, projectiles, and explosion effects are pooled with a simple PoolManager
9. Managers, such as WindowManager, is created to manage child entity such as UIWindow
10. Custom Hologram shader (from asset store) is used

Used Plugins:
1. DoTweenPro
2. Odin Inspector
3. Editor Console Pro
4. Build Report Tool
5. UniRX
6. UniTask

Used 3D Assets:
1. Low Poly Space Invader Set
2. Spaceship Collection Pack
3. 3D Fire and Explosions
4. Low-Poly UFO

Used 2D Assets:
1. GUI PRO Kit - Sci-Fi

Used Custom Shaders:
1. Sci-Fi Hologram Shader
