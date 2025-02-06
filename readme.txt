Jason Lei CS 4488 Project 1  ReadMe file

Hello TAs! Welcome to Jay city, a barren wasteland and excuse of a city (but one that fills all the requirements!). I'm providing this .txt file for ease of grading and accessibility.

Project version and file submission:
- The Unity version is 2022.3.43 (f1), and the project file submission is named lei_jason with Assets and ProjectSettings zipped up along with this file

Randomized Streets
- The project automatically creates a street at each runtime, using a modified version of recursive subdivision of up to 7 levels to divide a 100x100 grid
- Since recursive subdivision only accounts for shifting/adding streets and varying spacings, I added my own randomized code for random tile, whole street, and partial street deletion
- The randomization for recursive subdivision and street deletion is based off of a public random seed, accessible through the MainCamera
- Not every seed will provide streets that fill the requirement (for example, some will have isolated streets). In testing, it has been confirmed that seed 0 and seed 4 are guaranteed to provide results in which all streets are connected and all street types are shown
- There are smaller details added to ensure that the streets felt more realistic: for instance, streets that run off the edge of the grid are coded to appear as if it would connect to another 100x100 grid (should there be time for expansion), and recursive subdivision would not continue to run if the new slice made the streets too close to each other
- Most of this code is located in GenerateStreet.cs, attached to the MainCamera

Non-trivial Polygon Meshes
- All the roads and most road markings are generated through code adapted from the Warmup Project 2
- Specifically, it would be in the GenerateStraight.cs, GenerateTurn.cs, GenerateThreeway.cs, GenerateIntersection.cs, and GenerateRoundabout.cs files, attached to their respective street tile prefabs (the blank prefab does not have one for obvious reasons)
- The deadend prefab with the GenerateRoundabout.cs script has at least 32 vertices, which deals in creating two hexadecagon meshes: one for the road, and one for the cone divider in the middle (in total there are 34 unique vertices used)
- Each street tile has color variation in the form of the road and road marking colors (the road is dark grey, the markings are white), and even more variation if we consider 3D primitive objects
- There are a few situations in which Unity primitive objects are used: first would be the curb that outlines each street, these are 3D stretched cubes that were made directly in Unity, second would be the street markings seen in the 3-way and 4-way intersections for the crosswalks. While these would not constitute being made under triangle meshes as per the requirement, the core base of the road as well as every other road marking is still made with triangle meshes and thus each street tile does indeed fulfill that requirement (I used 3D objects here instead of coding the meshes for ease of design in order to make the streets look a lot better more efficiently)


Additional Requirements
- Camera movement has been included from the provided script attached to the MainCamera, which can be controlled front/back with W/S, and turn with A/D
- There are up to 15 buildings (made with 2 primitives) placed in each seed, with the guaranteed number of at least 6 being placed in the aforementioned seed 0 and seed 4

Extras
- There are up to 10 additional houses (shorter than buildings) placed in each seed as well
- There are up to 30 small trees placed in each seed
- The deadend has been redesigned to resemble a roundabout
- The 3-way intersection comes with stop signs for the crosswalk
- The 4-way intersection comes with traffic lights for the crosswalk
- Each street/road has a curb on each end to give more perceived volume and realism
- All of these extras (with the exception of the roundabout) have been made with Unity primitives

Thank you for reading up until this point! I hope this helps with grading and/or testing the project.