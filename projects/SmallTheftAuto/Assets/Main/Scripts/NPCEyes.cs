using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Seeing
{
    Nothing,
    Player,
    Enemy,
    NPC
}

public class NPCEyes : MonoBehaviour
{
    private Seeing seeing;
    private Transform PlayerTarget = null;
    
    public float maxDistance = 10f;
    
    [Range(0f, 360f)]
    public float angle = 45f;
    
    [SerializeField] bool visualize = true;
    
    public bool targetIsVisible { get; private set; }
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerTarget = GameObject.FindWithTag("ThePlayer").transform;
    }

    // Update is called once per frame
    void Update()
    {
       targetIsVisible = CheckVisibility();
       if (targetIsVisible)
       {
           GetComponentInChildren<NPCGunArm>().RaiseArm();
           GetComponentInChildren<NPCMovement>().relevantTransform = PlayerTarget;
           GetComponentInChildren<NPCMovement>().EnemySeen();
       }
       
    }
    
    public bool CheckVisibilityToPoint(Vector3 worldPoint) {

        // Calculate the direction from our location to the point
        var directionToTarget = worldPoint - transform.position;

        // Calculate the number of degrees from the forward direction.
        var degreesToTarget =
            Vector3.Angle(transform.forward, directionToTarget);

        // The target is within the arc if it's within half of the
        // specified angle. If it's not within the arc, it's not visible.
        var withinArc = degreesToTarget < (angle / 2);

        if (withinArc == false)
        {
            return false;
        }

        // Figure out the distance to the target
        var distanceToTarget = directionToTarget.magnitude;

        // Take into account our maximum distance
        var rayDistance = Mathf.Min(maxDistance, distanceToTarget);

        // Create a new ray that goes from our current location, in the
        // specified direction
        var ray = new Ray(transform.position, directionToTarget);

        // Stores information about anything we hit
        RaycastHit hit;

        // Perform the raycast. Did it hit anything?
        if (Physics.Raycast(ray, out hit, rayDistance)) {
            // We hit something.
            if (hit.collider.transform == PlayerTarget) {
                // It was the target itself. We can see the target point.
                return true;
            }
            // It's something between us and the target. We cannot see
            // the target point.
            return false;
        } else {
            // There's an unobstructed line of sight between us and the
            // target point, so we can see it.
            return true;
        }
    }

    // Returns true if a straight line can be drawn between this object
    // and the target. The target must be within range, and within the
    
     // visible arc.
     public bool CheckVisibility()
     {
         // Compute the direction to the target
         var directionToTarget = PlayerTarget.position - transform.position;

         // Calculate the number of degrees from the forward direction.
         var degreesToTarget =
             Vector3.Angle(transform.forward, directionToTarget);

         // The target is within the arc if it's within half of the
         // specified angle. If it's not within the arc, it's not visible.
         var withinArc = degreesToTarget < (angle / 2);

         if (withinArc == false)
         {
             return false;
         }

         // Compute the distance to the point
         var distanceToTarget = directionToTarget.magnitude;

         // Our ray should go as far as the target is or the maximum
         // distance, whichever is shorter
         var rayDistance = Mathf.Min(maxDistance, distanceToTarget);

         // Create a ray that fires out from our position to the target
         var ray = new Ray(transform.position, directionToTarget);

         // Store information about what was hit in this variable
         RaycastHit hit;

         // Records info about whether the target is in range and not
         // occluded
         var canSee = false;

         // Fire the raycast. Did it hit anything?
         if (Physics.Raycast(ray, out hit, rayDistance))
         {
             Debug.Log(hit.collider.name);
             // Did the ray hit our target?
             if (hit.collider.transform == PlayerTarget)
             {
                 // Then we can see it (that is, the ray didn't hit an
                 // obstacle in between us and the target)
                 canSee = true;
             }

             // Visualize the ray.
             Debug.DrawLine(transform.position, hit.point);

         }
         else
         {
             // The ray didn't hit anything. This means that it reached
             // the maximum distance and stopped, which means we didn't
             // hit our target. It must be out of range.

             // Visualize the ray.
             Debug.DrawRay(transform.position,
                 directionToTarget.normalized * rayDistance);
         }

         return canSee;
         // Is it visible?
     }
     
     
#if UNITY_EDITOR
[CustomEditor(typeof(NPCEyes))]
public class EnemyVisibilityEditor : Editor 
{
    private void OnSceneGUI()
    {
        // Get a reference to the EnemyVisibility script we're
        // looking at
        var visibility = target as NPCEyes;

        // Start drawing at 10% opacity
        Handles.color = new Color(1, 1, 1, 0.1f);

        // Drawing an arc sweeps from the point you give it. We want to
        // draw the arc such that the middle of the arc is in front of
        // the object, so we'll take the forward direction and rotate
        // it by half the angle.

        var forwardPointMinusHalfAngle =
            // rotate around the y-axis by half the angle
            Quaternion.Euler(0, -visibility.angle / 2, 0)
                      // rotate the forward direction by this
                      * visibility.transform.forward;

        // Draw the arc to visualize the visibility arc
        Vector3 arcStart =
            forwardPointMinusHalfAngle * visibility.maxDistance;

        Handles.DrawSolidArc(
            visibility.transform.position, // Center of the arc
            Vector3.up,                    // Up direction of the arc
            arcStart,                      // Point where it begins
            visibility.angle,              // Angle of the arc
            visibility.maxDistance         // Radius of the arc
        );

        // Draw a scale handle at the edge of the arc; if the user drags
        // it, update the arc size.

        // Reset the handle color to full opacity
        Handles.color = Color.white;

        // Compute the position of the handle, based on the object's
        // position, the direction it's facing, and the distance
        Vector3 handlePosition =
            visibility.transform.position +
                  visibility.transform.forward * visibility.maxDistance;

        // Draw the handle, and store its result.
        visibility.maxDistance = Handles.ScaleValueHandle(
            visibility.maxDistance,  
            // current value
            handlePosition,                 // handle position
            visibility.transform.rotation,  // orientation
            1,                              // size
            Handles.ConeHandleCap,          // cap to draw
            0.25f);                         // snap to multiples of this
        // if the snapping key is
        // held down
    }
}
#endif
}
