using UnityEngine;

public class CheckPoint : MonoBehaviour {

    public int numCheckpoint = 0;
    public Transform murInvisible;

    public static bool blockEscalier = false;

    private static int currentCheckpoint = 0;
    private static Transform currentTransform;

    void Start()
    {
        if (numCheckpoint == 0)
        {
            currentTransform = transform;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (numCheckpoint == 0 && !blockEscalier)
            {
                murInvisible.Translate(-5.5f, 0f, 0f);
                blockEscalier = true;
            }
            else if (numCheckpoint > currentCheckpoint)
            {
                currentCheckpoint = numCheckpoint;
                currentTransform = transform;
            }
        }
    }

    public static Vector3 GetCheckPointPosition()
    {
        return currentTransform.position;
    }

    public static Quaternion GetCheckPointRotation()
    {
        return currentTransform.rotation;
    }

    public static void Reset()
    {
        currentCheckpoint = 0;
        blockEscalier = false;
    }
}
