using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Piece : MonoBehaviour
{
    public string nomPiece;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FindObjectOfType<Radar>().EnterPiece(nomPiece);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            FindObjectOfType<Radar>().ExitPiece(nomPiece);
        }
    }
}