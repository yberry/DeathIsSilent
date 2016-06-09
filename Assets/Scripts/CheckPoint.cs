using UnityEngine;

public class CheckPoint : MonoBehaviour {

    public int numCheckpoint = 0;

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
            if (numCheckpoint > currentCheckpoint)
            {
                currentCheckpoint = numCheckpoint;
                currentTransform = transform;
            }
        }
    }

    public static Transform GetCheckPoint()
    {
        return currentTransform;
    }
}
