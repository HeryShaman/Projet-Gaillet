using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionManager : MonoBehaviour
{
    public GameObject infectionNodePrefab;
    public float spreadInterval = 2f;
    public float spreadRadius = 5f;
    public int maxNodes = 50;

    private List<InfectionNode> activeNodes = new List<InfectionNode>();

    void Start()
    {
        // Crée un premier point au centre du manager
        SpawnNode(transform.position);
        StartCoroutine(PropagationLoop());
    }

    IEnumerator PropagationLoop()
    {
        while (activeNodes.Count < maxNodes)
        {
            yield return new WaitForSeconds(spreadInterval);

            if (activeNodes.Count == 0) yield break;

            // Choisir un node aléatoire pour propager
            InfectionNode source = activeNodes[Random.Range(0, activeNodes.Count)];

            // Nouvelle position autour du node source
            Vector3 newPos = source.transform.position + Random.insideUnitSphere * spreadRadius;
            newPos.y = 0f; // garde au sol

            SpawnNode(newPos);
        }
    }

    void SpawnNode(Vector3 pos)
    {
        GameObject nodeObj = Instantiate(infectionNodePrefab, pos, Quaternion.identity);
        InfectionNode node = nodeObj.GetComponent<InfectionNode>();
        node.manager = this;
        activeNodes.Add(node);
    }

    public void RemoveNode(InfectionNode node)
    {
        activeNodes.Remove(node);
    }
}
