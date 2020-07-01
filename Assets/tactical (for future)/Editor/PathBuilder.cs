using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Walkable))/*, CanEditMultipleObjects*/]
public class PathBuilder : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Walkable originalScript = (Walkable)target;
        Walkable[] Tiles = FindObjectsOfType<Walkable>();


        if(GUILayout.Button("Calculate the pathways"))
        {
            originalScript.possiblePaths.Clear();
            foreach (Walkable tile in Tiles)
            {
                if (Vector3.Distance(originalScript.gameObject.transform.position, tile.gameObject.transform.position) <= 1.5f && originalScript != tile)
                {
                    //Vector3 rotationVector = Vector3.Normalize(originalScript.gameObject.transform.position - tile.gameObject.transform.position);
                    /*if(Vector3.Dot(originalScript.gameObject.transform.forward, rotationVector))
                    {

                    }*/
                    if(tile.gameObject.transform.position.x != originalScript.gameObject.transform.position.x && tile.gameObject.transform.position.z != originalScript.gameObject.transform.position.z)
                    {
                        Debug.Log("Diagonal");
                        if (!originalScript.possiblePaths.Exists(e => e.target == tile.gameObject.transform))
                        {
                            WalkPath wptemp = new WalkPath();
                            wptemp.active = true;
                            wptemp.target = tile.transform;
                            wptemp.diagonal = true;
                            originalScript.possiblePaths.Add(wptemp);
                        }
                        if (!tile.possiblePaths.Exists(e => e.target == originalScript.gameObject.transform))
                        {
                            WalkPath wptemp = new WalkPath();
                            wptemp.active = true;
                            wptemp.target = originalScript.transform;
                            wptemp.diagonal = true;
                            tile.possiblePaths.Add(wptemp);
                        }


                    }
                    else
                    {
                        Debug.Log("Straight");
                        if (!originalScript.possiblePaths.Exists(e => e.target == tile.gameObject.transform))
                        {
                            WalkPath wptemp = new WalkPath();
                            wptemp.active = true;
                            wptemp.target = tile.transform;
                            wptemp.diagonal = false;
                            originalScript.possiblePaths.Add(wptemp);
                        }
                        if (!tile.possiblePaths.Exists(e => e.target == originalScript.gameObject.transform))
                        {
                            WalkPath wptemp = new WalkPath();
                            wptemp.active = true;
                            wptemp.target = originalScript.transform;
                            wptemp.diagonal = false;
                            tile.possiblePaths.Add(wptemp);
                        }
                    }
                    //Debug.Log(Vector3.Dot(originalScript.gameObject.transform.forward, rotationVector));
                }
            }

            /*originalScript.possiblePaths.Clear();
            if(Physics.Raycast(new Ray(new Vector3(originalScript.transform.position.x + 1, originalScript.transform.position.y - 2, originalScript.transform.position.z), originalScript.transform.up), out RaycastHit hit1, 3.0f))
            {
                if(hit1.collider.gameObject.GetComponent<Walkable>() != null)
                {
                    AttemptAConnection(hit1.collider.gameObject.GetComponent<Walkable>(), originalScript, false);
                    //non-diagonal
                }
            }
            Debug.DrawRay(new Vector3(originalScript.transform.position.x + 1, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z), originalScript.transform.up * 3,Color.white);
            if(Physics.Raycast(new Ray(new Vector3(originalScript.transform.position.x + 1, originalScript.transform.position.y - 2, originalScript.transform.position.z + 1), originalScript.transform.up), out RaycastHit hit2, 3.0f))
            {
                if(hit2.collider.gameObject.GetComponent<Walkable>() != null)
                {
                    AttemptAConnection(hit2.collider.gameObject.GetComponent<Walkable>(), originalScript, true);
                }
            }
            Debug.DrawRay(new Vector3(originalScript.transform.position.x + 1, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z + 1), originalScript.transform.up * 3, Color.white);
            if(Physics.Raycast(new Ray(new Vector3(originalScript.transform.position.x, originalScript.transform.position.y - 2, originalScript.transform.position.z + 1), originalScript.transform.up),out RaycastHit hit3, 3.0f))
            {
                if(hit3.collider.gameObject.GetComponent<Walkable>() != null)
                {
                    AttemptAConnection(hit3.collider.gameObject.GetComponent<Walkable>(), originalScript, false);
                }
            }
            Debug.DrawRay(new Vector3(originalScript.transform.position.x, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z + 1), originalScript.transform.up * 3, Color.white);
            if (Physics.Raycast(new Ray(new Vector3(originalScript.transform.position.x - 1, originalScript.transform.position.y - 2, originalScript.transform.position.z + 1), originalScript.transform.up), out RaycastHit hit4, 3.0f))
            {
                if (hit4.collider.gameObject.GetComponent<Walkable>() != null)
                {
                    AttemptAConnection(hit4.collider.gameObject.GetComponent<Walkable>(), originalScript, true);
                }
            }
            Debug.DrawRay(new Vector3(originalScript.transform.position.x - 1, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z + 1), originalScript.transform.up * 3, Color.white);
            if (Physics.Raycast(new Ray(new Vector3(originalScript.transform.position.x - 1, originalScript.transform.position.y - 2, originalScript.transform.position.z), originalScript.transform.up), out RaycastHit hit5, 3.0f))
            {
                if (hit5.collider.gameObject.GetComponent<Walkable>() != null)
                {
                    AttemptAConnection(hit5.collider.gameObject.GetComponent<Walkable>(), originalScript, false);
                }
            }
            Debug.DrawRay(new Vector3(originalScript.transform.position.x - 1, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z), originalScript.transform.up * 3, Color.white);
            if (Physics.Raycast(new Ray(new Vector3(originalScript.transform.position.x - 1, originalScript.transform.position.y - 2, originalScript.transform.position.z - 1), originalScript.transform.up), out RaycastHit hit6, 3.0f))
            {
                if (hit6.collider.gameObject.GetComponent<Walkable>() != null)
                {
                    AttemptAConnection(hit6.collider.gameObject.GetComponent<Walkable>(), originalScript, true);
                }
            }
            Debug.DrawRay(new Vector3(originalScript.transform.position.x - 1, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z - 1), originalScript.transform.up * 3, Color.white);
            if (Physics.Raycast(new Ray(new Vector3(originalScript.transform.position.x, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z - 1), originalScript.transform.up), out RaycastHit hit7, 3.0f))
            {
                if (hit7.collider.gameObject.GetComponent<Walkable>() != null)
                {
                    AttemptAConnection(hit7.collider.gameObject.GetComponent<Walkable>(), originalScript, false);
                }
            }
            Debug.DrawRay(new Vector3(originalScript.transform.position.x, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z - 1), originalScript.transform.up * 3, Color.white);
            if (Physics.Raycast(new Ray(new Vector3(originalScript.transform.position.x + 1, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z - 1), originalScript.transform.up), out RaycastHit hit8, 3.0f))
            {
                if (hit8.collider.gameObject.GetComponent<Walkable>() != null)
                {
                    AttemptAConnection(hit8.collider.gameObject.GetComponent<Walkable>(), originalScript, true);
                }
            }
            Debug.DrawRay(new Vector3(originalScript.transform.position.x + 1, originalScript.transform.position.y - 1.5f, originalScript.transform.position.z - 1), originalScript.transform.up * 3, Color.white);*/
        }
    }
    void AttemptAConnection(Walkable walkable, Walkable ftarget, bool diagonal)
    {
        //Debug.Log(walkable.gameObject);
        //Debug.Log(ftarget);
        /*if (!walkable.PathGOs.Contains(ftarget.gameObject))
        {
            WalkPath wptemp = new WalkPath();
            wptemp.active = true;
            wptemp.target = ftarget.transform;
            wptemp.diagonal = diagonal;
            walkable.possiblePaths.Add(wptemp);
        }*/
        /*if(!ftarget.PathGOs.Contains(walkable.gameObject))
        {
            WalkPath wptemp2 = new WalkPath();
            wptemp2.target = walkable.transform;
            wptemp2.active = true;
            wptemp2.diagonal = diagonal;
            ftarget.possiblePaths.Add(wptemp2);
        }*/
            if (!walkable.possiblePaths.Exists(e => e.target == ftarget.gameObject.transform))
            {
                WalkPath wptemp = new WalkPath();
                wptemp.active = true;
                wptemp.target = ftarget.transform;
                wptemp.diagonal = diagonal;
                walkable.possiblePaths.Add(wptemp);
            }


            /*if (ftarget.possiblePaths.FindIndex(walk => walk.target == walkable.gameObject.transform) == -1)
            {
                WalkPath wptemp2 = new WalkPath();
                wptemp2.target = walkable.transform;
                wptemp2.active = true;
                wptemp2.diagonal = diagonal;
                ftarget.possiblePaths.Add(wptemp2);
            }*/

        
        return;
    }
}
