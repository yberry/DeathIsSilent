using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject murRadar;

    private Rect dimensions;

    void Start()
    {
        GameObject pieceRadar = new GameObject(transform.name + "-radar");
        pieceRadar.transform.position = transform.position;
        CreateWalls(pieceRadar.transform);
    }

    void CreateWalls(Transform tr)
    {
        GameObject nord = Instantiate(murRadar) as GameObject;
        nord.transform.localPosition = new Vector3(0f, 0f, dimensions.height / 2f);
        nord.transform.localScale = new Vector3(dimensions.width, 1f, 1f);
        nord.transform.SetParent(tr);

        GameObject sud = Instantiate(murRadar) as GameObject;
        sud.transform.localPosition = new Vector3(0f, 0f, - dimensions.height / 2f);
        sud.transform.localScale = new Vector3(dimensions.width, 1f, 1f);
        sud.transform.SetParent(tr);

        GameObject est = Instantiate(murRadar) as GameObject;
        est.transform.localPosition = new Vector3(dimensions.height / 2f, 0f, 0f);
        est.transform.localScale = new Vector3(1f, 1f, dimensions.width);
        est.transform.SetParent(tr);

        GameObject ouest = Instantiate(murRadar) as GameObject;
        ouest.transform.localPosition = new Vector3(- dimensions.height / 2f, 0f, 0f);
        ouest.transform.localScale = new Vector3(1f, 1f, dimensions.width);
        ouest.transform.SetParent(tr);
    }
}