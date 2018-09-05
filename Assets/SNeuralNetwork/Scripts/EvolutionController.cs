using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvolutionController : MonoBehaviour
{
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
    public Queue<int> fitnesses;
    public SNeuralNetwork[] fittestAgents;

    private float[][] nextGenerationCodes;


    /// <summary>
    /// The death timer, if 0 then bot dies.
    /// </summary>
    private float deathTimer = 5;

    private bool firstGeneration = true;
    private bool weakGenerations = false;
    private bool laterGenerations = false;
    private bool firstGenerationInitialization = false;
    private bool weakGenerationInitialization = false;
    private bool laterGenerationInitialization = false;



    private void Start()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        spawnPointsTransforms = new Transform[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPointsTransforms[i] = spawnPoints[i].transform;
        }

        NNAgents = new SNeuralNetwork[spawnPoints.Length];

        SpawnAgents();


        AddStatPanels();
        UpdatePanels();
    }

    private void Update()
    {

        if (firstGeneration)
        {

            Debug.Log(deathTimer);
            if (deathTimer <= 0)
            {
                GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
                fittestAgents = FindFittestBots(NNAgents, fitnesses.ToArray(), 4);
                for (int i = 0; i < bots.Length; i++)
                {
                    fitnesses.Enqueue(NNAgents[i].fitness);
                    Debug.Log("Destroying");
                    Destroy(bots[i]);
                    Debug.Log("Destroying2");

                }


                nextGenerationCodes = Breed(fittestAgents, 6);

                firstGeneration = false;
                weakGenerations = true;
            }
            else
            {
                deathTimer -= Time.deltaTime * 1;


            }
        }
        if (weakGenerations)
        {

            if (!weakGenerationInitialization)
            {
                GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
                spawnPointsTransforms = new Transform[spawnPoints.Length];
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    spawnPointsTransforms[i] = spawnPoints[i].transform;
                }

                NNAgents = new SNeuralNetwork[spawnPoints.Length];

                SpawnAgents(nextGenerationCodes);

                weakGenerationInitialization = true;
            }
            

            Debug.Log(deathTimer);
            if (deathTimer <= 0)
            {
                GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
                fittestAgents = FindFittestBots(NNAgents, fitnesses.ToArray(), 4);
                for (int i = 0; i < bots.Length; i++)
                {
                    fitnesses.Enqueue(NNAgents[i].fitness);
                    Destroy(bots[i]);
                }


                nextGenerationCodes = Breed(fittestAgents, 6);

                firstGeneration = false;
                weakGenerations = true;
            }
            else
            {
                deathTimer -= Time.deltaTime * 1;


            }
        }
        if (laterGenerations)
        {

        }

        UpdatePanels();
    }

    private void AddStatPanels()
    {
        activePanels = new GameObject[NNAgents.Length];
        GameObject panelToSpawnIn = statsDisplayPanelLeft;
        for (int i = 0; i < NNAgents.Length; i++)
        {
            activePanels[i] = Instantiate(statsPrefab, panelToSpawnIn.transform, false);

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

            StatPanel statPanel = activePanels[i].GetComponent<StatPanel>();

            statPanel.title.text = NNAgents[i].name.ToString() + " " + i;
            statPanel.fitness.text = NNAgents[i].fitness.ToString();
            //Debug.Log(NNAgents[i].NN.ReadGeneticCode);
            statPanel.geneticCode.text = NNAgents[i].NN.ReadGeneticCode;

        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="requiredAmount"></param>
    /// <returns>
    /// this is the amount of bots you will return
    /// </returns>
    private SNeuralNetwork[] FindFittestBots(SNeuralNetwork[] inputAgentArray, int[] fitnessArray, int requiredAmount)
    {
        SNeuralNetwork[] fittestAgents = new SNeuralNetwork[requiredAmount];

        for (int i = 0; i < fittestAgents.Length; i++)
        {
            int maxIndex = fitnessArray.ToList().IndexOf(fitnessArray.Max());
            fittestAgents[i] = inputAgentArray[maxIndex];
            fitnessArray[maxIndex] = 0;
            Debug.Log(fitnessArray);
        }
        Debug.Log(fittestAgents);
        return fittestAgents;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputParents">
    /// Has to be only 4
    /// </param>
    /// <param name="alleleDivision">
    /// make sure that This is right amount (has to divde the genetic code length without leftovers)
    /// </param>
    /// <returns> </returns>
    private float[][] Breed(SNeuralNetwork[] inputParents, int alleleDivision)
    {
        float[][] childAgents = new float[12][];

        string[] childAgentsString = new string[12];

        //72 / 12 = 6 for each .Each of those should be mutated. for each 6 indexes 1 of those parts should be mutated
        childAgentsString = GeneticCrossover(inputParents, alleleDivision);


        childAgentsString = Mutate(childAgentsString, alleleDivision);

        for (int i = 0; i < childAgents.Length; i++)
        {
            childAgents[i] = Array.ConvertAll(childAgentsString[i].Split(','), float.Parse);
        }
        return childAgents; 

        
    }

    private string[] GeneticCrossover(SNeuralNetwork[] inputParents,int alleleDivision)
    {
        float[][] crossoverAgents = new float[12][];
        string[] crossoverOutcome = new string[12];
        int arrayLength = inputParents[0].NN.ReadFloatGeneticCode.Length;

        crossoverAgents[0] = inputParents[0].NN.ReadFloatGeneticCode;
        crossoverAgents[1] = inputParents[1].NN.ReadFloatGeneticCode;

        int counter = 0;
        int arrayCreateCounter = 0;
        for (int i = 0; i < inputParents.Length; i++)
        {
            for (int ii = i + 1; ii < inputParents.Length; ii++)
            {
                if (UnityEngine.Random.value < 0.5)
                {
                    crossoverAgents[arrayCreateCounter] = inputParents[i].NN.ReadFloatGeneticCode;
                    arrayCreateCounter++;
                    crossoverAgents[arrayCreateCounter] = inputParents[ii].NN.ReadFloatGeneticCode;
                    arrayCreateCounter++;

                    for (int iii = 0; iii <= arrayLength; iii+= alleleDivision)
                    {
                        Array.Copy(crossoverAgents[arrayCreateCounter - 1],iii, crossoverAgents[arrayCreateCounter - 2],iii,alleleDivision);
                    }
                    counter += 2;
                    if (counter >= 12)
                    {
                        goto exit;
                    }
                }
            }
            
        }
        exit:

        for (int i = 0; i < crossoverOutcome.Length; i++)
        {
            crossoverOutcome[i] = string.Join(",", crossoverAgents[i]);

        }

        return crossoverOutcome;

    }

    /// <summary>
    /// Returns a mutated NeuralNetwork
    /// </summary>
    /// <param name="inputParents"></param>
    /// The NN that will be mutated
    /// <param name="alelleDivision"></param>
    /// how many indexes there will be per allele
    /// <returns>Mutated NN</returns>
    private string[] Mutate (string[] inputNNCode,int alleleDivision)
    {


        float[][] geneticCodeToMutate = new float[12][];
        string[] mutationOutcome = new string[12];


        for (int i = 0; i < geneticCodeToMutate.Length; i++)
        {
            geneticCodeToMutate[i] = Array.ConvertAll(inputNNCode[i].Split(','), float.Parse);
        }

        for (int ii = 0; ii < geneticCodeToMutate[0].Length; ii++)
        {
            for (int i = 0; i <= geneticCodeToMutate[0].Length / alleleDivision; i++)
            {
                geneticCodeToMutate[i][UnityEngine.Random.Range(alleleDivision * ii, (ii + 1) * 5)] = UnityEngine.Random.Range(-8f, 8f);
            }
        }
        

        for (int i = 0; i < geneticCodeToMutate.Length; i++)
        {
            mutationOutcome[i] = string.Join(",", geneticCodeToMutate[i]);

        }

        return mutationOutcome;
    }

    private void SpawnAgents()
    {
        for (int i = 1; i < spawnPointsTransforms.Length; i += 2)
        {
            GameObject firstBot = Instantiate(botPrefab, spawnPointsTransforms[i - 1].position, spawnPointsTransforms[i - 1].rotation);
            GameObject secondBot = Instantiate(botPrefab, spawnPointsTransforms[i].position, spawnPointsTransforms[i].rotation);

            firstBot.GetComponent<BotController>().Opponent = secondBot.GetComponent<BotController>();
            secondBot.GetComponent<BotController>().Opponent = firstBot.GetComponent<BotController>();

            NNAgents[i - 1] = firstBot.GetComponent<SNeuralNetwork>();
            NNAgents[i] = secondBot.GetComponent<SNeuralNetwork>();
        }
    }

    private void SpawnAgents(float[][] geneticCodes)
    {
        for (int i = 1; i < spawnPointsTransforms.Length; i += 2)
        {
            GameObject firstBot = Instantiate(botPrefab, spawnPointsTransforms[i - 1].position, spawnPointsTransforms[i - 1].rotation);
            GameObject secondBot = Instantiate(botPrefab, spawnPointsTransforms[i].position, spawnPointsTransforms[i].rotation);

            firstBot.GetComponent<BotController>().Opponent = secondBot.GetComponent<BotController>();
            secondBot.GetComponent<BotController>().Opponent = firstBot.GetComponent<BotController>();

            NNAgents[i - 1] = firstBot.GetComponent<SNeuralNetwork>();
            NNAgents[i - 1].NN.SetGeneticCode(geneticCodes[i - 1]);
            NNAgents[i] = secondBot.GetComponent<SNeuralNetwork>();
            NNAgents[i].NN.SetGeneticCode(geneticCodes[i]);

        }
    }



    
}