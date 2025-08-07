using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SacrificeTableScript : MonoBehaviour
{
    public List<GameObject> targets;
    public List<GameObject> objects;
    public TextMeshProUGUI score;
    public float wiggleValue;

    private void Start()
    {
        Dependencies.Instance.RegisterDependency<SacrificeTableScript>(this);
    }

    private void OnEnable()
    {
        foreach (var _object in objects)
        {
            _object.transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-1.5f, 1.5f), 1, 0), Quaternion.identity);
        }
    }
    public void ChechTable()
    {
        score.text = "0/4";
        int currentScore = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            float currentDistance = Vector3.Distance(targets[i].transform.position, objects[i].transform.position);
            if (currentDistance <= wiggleValue) currentScore++;
        }

        score.text = currentScore.ToString()+ "/4";
    }
}
