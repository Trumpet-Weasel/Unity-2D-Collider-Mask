# Unity-2D-Collider-Mask
A script that allows for the masking of a PolygonCollider2D in the Unity game engine.

This is a small script I threw together that uses <a href="https://github.com/AngusJohnson/Clipper2">Angus Johnsons Clipper2 Library</a> to mask a PolygonCollider2D.

The script takes three different colliders: <br>
<ul>
<li>Subject Collider - The base collider that represents the full collider. <br>
<li>Clipping Collider - The collider that acts as a mask for the subject collider. <br>
<li>Output Collider - The collider that the masked collider is output to. <br>
</ul>

A Unity project is attached showcasing two examples of where this script can be used: <br>

Magic Bridge - A platformer demo where you need to use a "magic bridge" to get through a barrier. <br>
TARDIS Effect - A demo containing a box that is larger on the inside than it is on the outside.

