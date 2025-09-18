using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzleComplitionCounter : MonoBehaviour
{
    public GameObject winScreen;
    public TextMeshProUGUI scoreText;
    public List<GameObject> puzzlePieces;
    public List<GameObject> puzzlePlacement;
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
        piecesCount = 0;
        scoreText.text = piecesCount.ToString() + "/7";
    }
    public void CheckPuzzle()
    {
        piecesCount = 0;
        int i = 0;
        foreach (var piece in puzzlePieces)
        {

            float currentDistance = Vector3.Distance(piece.transform.localPosition, puzzlePlacement[i].transform.localPosition);
            if (currentDistance < maxWiggleValue)
            {
                piecesCount++;
            }
            i++;
        }

        scoreText.text = piecesCount.ToString() + "/7";
        if (piecesCount == 7)
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
