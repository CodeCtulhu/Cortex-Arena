using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionController : MonoBehaviour {

    [SerializeField]
    private GameObject botPrefab;

    [SerializeField]
    internal GameObject statsDisplayPanelLeft;
    [SerializeField]
    internal GameObject statsDisplayPanelRight;
    [SerializeField]
    internal GameObject statsPrefab;

    internal GameObject[] activePanels;


    public SNeuralNetwork[] NNAgents;
    public Transform[] spawnPointsTransforms;


    private void Start()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        spawnPointsTransforms = new Transform[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPointsTransforms[i] = spawnPoints[i].transform;
        }

        
        NNAgents = new SNeuralNetwork[spawnPoints.Length];

        for (int i = 1; i < spawnPointsTransforms.Length; i += 2)
        {
            
            GameObject firstBot = Instantiate(botPrefab, spawnPointsTransforms[i - 1].position, spawnPointsTransforms[i - 1].rotation);
            GameObject secondBot = Instantiate(botPrefab, spawnPointsTransforms[i].position, spawnPointsTransforms[i].rotation);

            firstBot.GetComponent<BotController>().Opponent = secondBot.GetComponent<BotController>();
            secondBot.GetComponent<BotController>().Opponent = firstBot.GetComponent<BotController>();

            NNAgents[i - 1] = firstBot.GetComponent<SNeuralNetwork>();
            NNAgents[i] = secondBot.GetComponent<SNeuralNetwork>();

        }

        AddStatPanels();
        UpdatePanels();
    }


    private void Update()
    {
        UpdatePanels();
    }

    private void AddStatPanels()
    {
        activePanels = new GameObject[NNAgents.Length];
        GameObject panelToSpawnIn = statsDisplayPanelLeft;
        for (int i = 0; i < NNAgents.Length; i++)
        {
            activePanels[i] = Instantiate(statsPrefab, panelToSpawnIn.transform,false);
            
            if (i >= 5)
            {
                panelToSpawnIn = statsDisplayPanelRight;
            }
            if (i >= 11)
            {
                return;
            }
        }
    }

    private void UpdatePanels()
    {
        for (int i = 0; i < NNAgents.Length; i++)
        {
            StatPanel statPanel= activePanels[i].GetComponent<StatPanel>();

            statPanel.title.text = NNAgents[i].name.ToString() + " " + i;
            statPanel.fitness.text = NNAgents[i].fitness.ToString();
            statPanel.geneticCode.text = NNAgents[i].NN.ReadGeneticCode;

            Debug.Log(NNAgents[i]);
            Debug.Log(NNAgents[i].NN);

        }
    }
}
