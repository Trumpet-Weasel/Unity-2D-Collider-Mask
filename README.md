# Unity-2D-Collider-Mask
A script (PolygonClipper) that allows for the masking of a PolygonCollider2D in the Unity game engine. Much like a <a href="https://docs.unity3d.com/ScriptReference/SpriteMask.html">sprite mask</a>.

The script uses <a href="https://github.com/AngusJohnson/Clipper2">Angus Johnsons Clipper2 Library</a> to get the intersection/difference of two intersecting polygon collider's, and output's the result to another polygon collider.

The script takes three different colliders: <br>
<ul>
<li>Subject Collider - The base collider that represents the full collider. <br>
<li>Clipping Collider - The collider that acts as a mask for the subject collider. <br>
<li>Output Collider - The collider that the masked collider is output to. <br>
</ul>

The script is kind of bare bones on it's own, but can be used to create some pretty cool stuff. I uploaded a Unity Project (ColliderMaskDemos) showcasing two examples of projects where the script could be used: <br>
<ul>
<li>Magic Bridge - A platformer demo where you need to use a "magic bridge" to get through a barrier. <br>
<li>TARDIS Effect - A demo containing a box that is larger on the inside than it is on the outside. <br>
</ul>

I threw this together for a personal project, but decided to put it on Github hoping somebody else would find it useful. Hope you find a cool way to use this!
