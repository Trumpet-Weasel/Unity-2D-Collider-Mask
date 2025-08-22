using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


//A script that automatically adds clipping colliders to an object, and assigns them for interaction with hidden space
public class MakeObjectClippable : MonoBehaviour
{
    GameObject NormalClipper, HiddenClipper;
    public bool InHiddenSpace; //Change this before runtime if you want an object to start in hidden space
    bool InMask;
    bool InGate;

    private void Start()
    {
        if(NormalClipper == null)
        {
            //Creates and assigns values to the "normal" collider
            NormalClipper = new GameObject();
            NormalClipper.name = "Normal Col";
            NormalClipper.transform.position = transform.position;
            NormalClipper.transform.parent = transform;
            Helpers.CopyPoints(GetComponent<PolygonCollider2D>(), NormalClipper.AddComponent<PolygonCollider2D>());

            PolygonClipper TPPCNormal = NormalClipper.AddComponent<PolygonClipper>();

            TPPCNormal.subjCol = GetComponent<PolygonCollider2D>();
            TPPCNormal.outputCol = NormalClipper.GetComponent<PolygonCollider2D>();
            TPPCNormal.MaskInteraction = PolygonClipper.MaskInteractionOps.VisibleOutsideMask;
        }

        if(HiddenClipper == null)
        {
            //Creates and assigns values to the collider that interacts with hidden space.
            HiddenClipper = new GameObject();
            HiddenClipper.name = "Non-Eucledian Col";
            HiddenClipper.transform.position = transform.position;
            HiddenClipper.transform.parent = transform;
            HiddenClipper.AddComponent<PolygonCollider2D>().pathCount = 0;
            HiddenClipper.layer = LayerMask.NameToLayer("NonEucledianObj");

            PolygonClipper TPPCNonEucledian = HiddenClipper.AddComponent<PolygonClipper>();

            TPPCNonEucledian.subjCol = GetComponent<PolygonCollider2D>();
            TPPCNonEucledian.outputCol = TPPCNonEucledian.GetComponent<PolygonCollider2D>();
            TPPCNonEucledian.MaskInteraction = PolygonClipper.MaskInteractionOps.VisibleInsideMask;
        }

       //If the object is marked as starting in hidden space, get the clip collider, and assign it.
        if (InHiddenSpace)
        {
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            List<Collider2D> results = new List<Collider2D>();
            int count = GetComponent<PolygonCollider2D>().OverlapCollider(filter, results);

            Debug.Log(results.Count);
            for (int i = 0; i < count; i++)
            {
                if (results[i].GetComponent<ColliderMaskHolder>() != null)
                {
                    Debug.Log(results[i].name);
                    InMask = true;
                    Helpers.CopyPoints(GetComponent<PolygonCollider2D>(), HiddenClipper.GetComponent<PolygonCollider2D>());
                    NormalClipper.GetComponent<PolygonCollider2D>().pathCount = 0;
                    NormalClipper.GetComponent<PolygonClipper>().clipCol = results[i].GetComponent<ColliderMaskHolder>().ClipCol;
                    HiddenClipper.GetComponent<PolygonClipper>().clipCol = results[i].GetComponent<ColliderMaskHolder>().ClipCol;
                    break;

                }
            }

        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If the object enters a collider mask gate, assign clipCol.
        if (other.GetComponent<ColliderMaskGate>())
        {
            NormalClipper.GetComponent<PolygonClipper>().clipCol = other.GetComponent<ColliderMaskGate>().ClipCol;
            HiddenClipper.GetComponent<PolygonClipper>().clipCol = other.GetComponent<ColliderMaskGate>().ClipCol;
            InGate = true;
        }

        //Check if the object enters a sprite mask (hidden space)
        if (other.GetComponent<SpriteMask>())
        {
            InMask = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Checks if the object exits a gate/is inside of a mask
        if (other.GetComponent<ColliderMaskGate>())
        {
            InGate = false;
        }
        if (other.GetComponent<SpriteMask>())
        {
            InMask = false;
        }
    }

    private void Update()
    {
        if(InGate)
        {
            InHiddenSpace = true;
        }
        else if (!InMask && !InGate)
        {
            InHiddenSpace = false;
        }

        //Unassigns the clipCol if the object is in hidden space.
        if (!InHiddenSpace)
        {
            NormalClipper.GetComponent<PolygonClipper>().clipCol = null;
            HiddenClipper.GetComponent<PolygonClipper>().clipCol = null;
        }

        //Debugs warning messages if not adhering to assumptions
        if (GetComponent<PolygonCollider2D>() == false)
        {
            Debug.LogWarning(gameObject.name + ". Does not have a PolygonCollider2D attached. This object will experience unpredictable behaviour.");
        }
        if (GetComponent<SpriteRenderer>() == false && GetComponent<SpriteShapeRenderer>() == false)
        {
            Debug.LogWarning(gameObject.name + ". Does not have a SpriteRenderer or a Sprite Shape Renrerer attached. This object will experience unpredictable behaviour.");
        }

        //Assigns mask interaction depending on whether the object is in hidden space
        if (InHiddenSpace)
        {
            GetComponent<PolygonCollider2D>().isTrigger = true;
            if (UsingSSR())
            {
                GetComponent<SpriteShapeRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
        }
        else
        {
            GetComponent<PolygonCollider2D>().isTrigger = false;
            if (UsingSSR())
            {
                GetComponent<SpriteShapeRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
            else
            {
                GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
          
        }
    }


    bool UsingSSR()
    {
        return GetComponent<SpriteShapeRenderer>();
    }
}
