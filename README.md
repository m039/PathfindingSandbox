# Pathfinding Testbed

This project contains a demo to test benchmarks of some A\* search algorithms in Unity. 

[You can try it in a browser](http://m039.github.io/PathfindingTestbed), but it gives inaccurate results. It is better to use a desktop build.

Algorithms used:
* [Pathfinding by m039](https://github.com/m039/CommonUnityLibrary/tree/16189a32a353a77fd8ca36687641e57c09dbaf89/Runtime/Scripts/AI/Pathfinding) (from [Common Unit Library](https://github.com/m039/CommonUnityLibrary))
* [A\* Pathfinding Project](https://arongranberg.com/astar/)
* [Pathfinding by RonenNess](https://github.com/RonenNess/Unity-2d-pathfinding)

## Algorithms Performance

| Algorithm|Execution Time|
|---|---|
|m039's Pathfinding|1.3 ms.|
|A\* Pathfinding Project|1.7 ms.|
|RonenNess's Pathfindg|5 ms.|

You can find test case in the project. In short, I tested algorithms to find a path in one level 100 times and took the average time.

m039's Pathfinding is slightly faster than A* Pathfinding Project, but they are relatively the same speed. RonenNess's Pathfinding is several times slower than other algorithms and shows worst performance on open areas (because it doesn't use a priority queue).
