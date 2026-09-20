using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    public List<Rigidbody> allParts = new List<Rigidbody>();

    public AudioClip woodShatterClip;

    public void Shatter()   // Called by BulletScript when a bullet hits the crate. Enables physics on all fractured pieces, causing the crate to break apart.
    {
        GetComponent<BoxCollider>().enabled = false;    // Disable the parent Crate's Box Collider so the broken pieces can fall freely without being blocked by the original collider.
        foreach (Rigidbody part in allParts)
        {
            part.isKinematic = false;   //allowing physics and gravity
        }
        AudioManager.Instance.PlayWoodShatterSound(woodShatterClip);
    }
}




// Setup Instructions:
//
// 1. Drag and drop the destructible crate prefab into the scene. Also Set "Crate" tag for parent crate.
//
// 2. Select the parent Crate object:
//    - Attach the CrateScript.
//    - Add a Box Collider.
//    - Click "Edit Collider" and resize it so that it slightly surrounds the entire crate.
//      This collider is used to detect bullet hits.(Slightly large than object)
//
// 3. In the CrateScript component:
//    - Populate the "All Parts" list by dragging all the fractured pieces (with Rigidbodies)
//      from the Hierarchy into the Inspector.
//
// 4. Select all fractured pieces:
//    - Add a Mesh Collider.
//    - Enable the "Convex" option on each Mesh Collider. => When Convex is checked, Unity creates a simplified, solid version of the mesh collider.
//    - Add a Rigidbody component.
//    - Enable "Is Kinematic" so the pieces remain stationary until the crate is destroyed. (object ignores physics)
//
// 5. When Shatter() is called:
//    - Each piece's Rigidbody becomes non-kinematic, allowing physics and gravity
//      to make the crate break apart naturally.