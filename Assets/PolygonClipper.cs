using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//YOU WILL NEED TO INSTALL CLIPPER2 FOR THIS SCRIPT TO WORK!!!
using Clipper2Lib;

/// <summary>
/// The script takes all of the points of a PolygonCollider2D, increases them by a scale (because the clipper library can only use vector2Int), uses Angus Johnson's clipper library to get the difference, scales the clip back down, and plugs it back into the polygon collider 2D
/// </summary>


public class PolygonClipper : MonoBehaviour
{
    //[Header("Assigning")]
    public PolygonCollider2D subjCol; //The normal reference collider 2D (Used for reference, should strictly be IsTrigger) (The cookie dough)
    public PolygonCollider2D clipCol; //The collider 2D that acts as a mask (Should be istrigger) (The cookie cutter).
    //I am beginning to notice that only allowing a single clipCol to mask the object is inefficient practice. I might add the option to assign more in an update.

    public PolygonCollider2D outputCol; //The collider 2D to output the masked collider to (Should not be istrigger) (The cut cookie dough)
    public enum MaskInteractionOps {VisibleInsideMask, VisibleOutsideMask }
    public MaskInteractionOps MaskInteraction; //Is output col visible inside or outside of the clipCol?

    //Debuggers that show the points in a collider
    //[Header("Debugging")]
    List<Path> debugSubjPoints = new List<Path>();
    List<Path> debugClipPoints = new List<Path>();
    List<Path> debugClippedPoints = new List<Path>();

    //Clipper2 strictly uses Vector2Int variables. This is pretty bad, as Unity space is continuous by default, and in all likelihood, an object's position will not be in a integer position.
    //To counteract this, I scale the position of the PolygonCollider points using a really big number to turn them into int values, perform the cut, and divide the points back down again.
    float scale = 1000000;

    // Start is called before the first frame update
    void Update()
    {
       if(clipCol != null && subjCol != null && outputCol != null)
        {
            UpdateTheClip();
        }
    }

    public void UpdateTheClip()
    {
        //Get the polygon collider points and place them into lists of vector2's, floats, and scaled integers.
        List<Path> subjPoints = PolygonColliderPointsToPointData(subjCol);
        List<Path> clipPoints = PolygonColliderPointsToPointData(clipCol);

        //Assign Debugging info
        debugSubjPoints = subjPoints;
        debugClipPoints = clipPoints;

        //Turn the points into paths64 values (The thing clipperLib uses)
        Paths64 subj = new Paths64();
        Paths64 clip = new Paths64();
        foreach (Path P in subjPoints)
        {
            subj.Add(Clipper.MakePath(P.scaledPoints.ToArray()));
        }

        foreach (Path P in clipPoints)
        {
            clip.Add(Clipper.MakePath(P.scaledPoints.ToArray()));
        }

        //Performs a clip
        Paths64 solution = new Paths64();
        if (MaskInteraction == MaskInteractionOps.VisibleOutsideMask)
        {
            solution = Clipper.Difference(subj, clip, FillRule.NonZero);
        }
        else
        {
            solution = Clipper.Intersect(subj, clip, FillRule.NonZero);
        }

        //Puts the cut points onto another collider
        List<List<Vector2>> pointsAfterClip = Paths64ToListListVector2(solution);

        AddPointsToPolygonCollider(pointsAfterClip, outputCol);
    }

    private List<Vector2> m_Path = new List<Vector2>();

    //Extracts all of the points from a polygon collider 2D, and turns them into a list of paths
    List<Path> PolygonColliderPointsToPointData(PolygonCollider2D PC)
    {
        List<Path> ListToReturn = new List<Path>();

        for (int i = 0; i < PC.pathCount; i++)
        {
            m_Path.Clear();

            PC.GetPath(i, m_Path);

            Path currentPath = new Path();

            foreach (var point in m_Path)
            {
                var worldPoint = PC.transform.localToWorldMatrix.MultiplyPoint(point);
                currentPath.PointsAsVector2.Add(worldPoint);
            }

            ListToReturn.Add(currentPath);
        }

        for (int i = 0; i < ListToReturn.Count; i++)
        {
            foreach(Vector2 V in ListToReturn[i].PointsAsVector2)
            {
                ListToReturn[i].pointsAsFloats.Add(V.x);
                ListToReturn[i].pointsAsFloats.Add(V.y);
            }
        }

        for (int i = 0; i < ListToReturn.Count; i++)
        {
            foreach(float f in ListToReturn[i].pointsAsFloats)
            {
                ListToReturn[i].scaledPoints.Add((int)(f * scale));
            }
        }

        return ListToReturn;
    }

    //A class containing the points as vector2's, the points as a list of floats, and a list of scaled ints.
    [System.Serializable]
    public class Path
    {
        public List<Vector2> PointsAsVector2 = new List<Vector2>();
        public List<float> pointsAsFloats = new List<float>();
        public List<int> scaledPoints = new List<int>();
    }

    //Function that converts a Paths64 (that contain scaled points) into a list containing lists containing vectors 
    List<List<Vector2>> Paths64ToListListVector2(Paths64 p)
    {
        List<List<Vector2>> listToReturn = new List<List<Vector2>>();
        for(int i = 0; i < p.Count; i++)
        {
            List<Vector2> pathList = new List<Vector2>();
            Path64 path = p[i];
            foreach(Point64 point in path)
            {
                pathList.Add(new Vector2(point.X / scale, point.Y / scale));
            }
            listToReturn.Add(pathList);
        }
        return listToReturn;
    }

    //Applies a list of lists of vector2's to a polygon collider 2D
    void AddPointsToPolygonCollider(List<List<Vector2>> points, PolygonCollider2D PC)
    {
        PC.pathCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            List<Vector2> worldPath = points[i];
            Vector2[] localPath = new Vector2[worldPath.Count];

            for (int j = 0; j < worldPath.Count; j++)
            {
                localPath[j] = transform.InverseTransformPoint(worldPath[j]);
            }

            PC.SetPath(i, localPath);
        }
    }

}

