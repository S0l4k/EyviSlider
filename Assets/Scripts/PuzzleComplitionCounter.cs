using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PuzzleComplitionCounter : MonoBehaviour
{
    public GameObject winScreen;
    public TextMeshProUGUI scoreText;
    public List<GameObject> puzzlePieces;
    public float maxWiggleValue = 1;
    public float dispersionValue = 1;
    public int piecesCount = 0;
    void Start()
    {
        Dependencies.Instance.RegisterDependency<PuzzleComplitionCounter>(this);
    }

    private void OnEnable()
    {
        foreach (var piece in puzzlePieces)
        {
            piece.transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-dispersionValue, dispersionValue), Random.Range(-dispersionValue, dispersionValue), 0), Quaternion.identity);
        }
    }
    public void CheckPuzzle()
    {
        piecesCount = 0;
        foreach (var piece in puzzlePieces)
        {
            if (piece.transform.localPosition.magnitude < maxWiggleValue)
            {
                piecesCount++;
            }
        }

        scoreText.text = piecesCount.ToString() + "/8";
        if (piecesCount == 8)
        {
            StartCoroutine(winning());
        }
    }

    public IEnumerator winning()
    {
        yield return new WaitForSeconds(2);

        winScreen.SetActive(true);
        PeruMinigamemanager PeruMainGame = Dependencies.Instance.GetDependancy<PeruMinigamemanager>();
        PeruMainGame.Minigame1 = true;
    }
}
