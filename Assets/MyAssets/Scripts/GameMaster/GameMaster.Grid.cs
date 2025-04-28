using UnityEngine;

public partial class GameMaster
{

    void CreateGrid()
    {
        for (int i = 0; i < groundPiece.localScale.x; i++)
        {
            for (int j = 0; j < groundPiece.localScale.z; j++)
            {
                grid.Add(Instantiate(gridPiece, new Vector3(i + 0.5f, 0.1f, j + 0.5f), Quaternion.identity));
            }
        }
    }

    void DeleteGrid()
    {
        foreach (GameObject gameObject in grid)
        {
            Destroy(gameObject);
        }
    }

}