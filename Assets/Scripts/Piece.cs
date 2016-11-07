using UnityEngine;

public class Piece : MonoBehaviour
{
    public string nomPiece;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {

        }
    }
}