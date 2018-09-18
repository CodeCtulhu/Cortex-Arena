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
    public Queue<int> fitnesses = new Queue<int>();
    [SerializeField]
    public float[][] fittestAgents;

    private float[][] nextGenerationCodes;


    /// <summary>
    /// The death timer, if 0 then bot dies.
    /// </summary>
    private float deathTimer = 10;

    private bool firstGeneration = false;
    private bool weakGenerations = false;
    private bool laterGenerations = false;
    private bool firstGenerationInitialization = false;
    private bool weakGenerationInitialization = false;
    private bool laterGenerationInitialization = false;



    private void Awake()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        spawnPointsTransforms = new Transform[spawnPoints.Length];
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPointsTransforms[i] = spawnPoints[i].transform;
        }

        NNAgents = new SNeuralNetwork[spawnPoints.Length];

        SpawnAgents();

        firstGeneration = true;

        AddStatPanels();
        UpdatePanels();
    }

    private void Update()
    {

        if (firstGeneration)
        {

            //Debug.Log("timer 1: " + deathTimer);
            if (deathTimer <= 0)
            {


                GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");

                for (int i = 0; i < bots.Length; i++)
                {
                    fitnesses.Enqueue(NNAgents[i].fitness);
                }

                fittestAgents = FindFittestBots(NNAgents, fitnesses.ToArray(), 4);
                fitnesses.Clear();




                for (int i = 0; i < bots.Length; i++)
                {

                    Destroy(bots[i]);


                }



                nextGenerationCodes = Breed(fittestAgents, 6);

                firstGeneration = false;
                weakGenerations = true;
            }
            else
            {
                deathTimer -= Time.deltaTime;


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
                deathTimer = 1000005;
            }


            //Debug.Log("timer 2: " + deathTimer);
            if (deathTimer <= 0)
            {
                GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
                for (int i = 0; i < bots.Length; i++)
                {

                    fitnesses.Enqueue(NNAgents[i].fitness);
                    Destroy(bots[i]);
                }

                fittestAgents = FindFittestBots(NNAgents, fitnesses.ToArray(), 4);
                fitnesses.Clear();

                nextGenerationCodes = Breed(fittestAgents, 6);

                firstGeneration = false;
                weakGenerations = true;
                weakGenerationInitialization = false;
            }
            else
            {
                deathTimer -= Time.deltaTime * 1;


            }
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
    private float[][] FindFittestBots(SNeuralNetwork[] inputAgentArray, int[] fitnessArray, int requiredAmount)
    {
        float[][] fittestAgents = new float[requiredAmount][];
        float[][] agentCodeArray = new float[inputAgentArray.Length][];

        for (int i = 0; i < agentCodeArray.Length; i++)
        {
            agentCodeArray[i] = inputAgentArray[i].NN.ReadFloatGeneticCode;
        }


        for (int i = 0; i < fittestAgents.Length; i++)
        {
            int value = fitnessArray.Max();
            int maxIndex = fitnessArray.ToList().IndexOf(value);

            List<int> maxIndexes =
                fitnessArray.Select((s, ix) => new { ix, s })
                    .Where(t => t.s == value)
                    .Select(t => t.ix)
                    .ToList();


            Debug.Log(fitnessArray.Length);



            if (maxIndexes.Count > 1)
            {

                maxIndex = maxIndexes[UnityEngine.Random.Range(0,maxIndexes.Count)];

            }




            fittestAgents[i] = agentCodeArray[maxIndex];

            fitnessArray[maxIndex] = int.MinValue;


        }

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
    private float[][] Breed(float[][] inputParents, int alleleDivision)
    {
        float[][] childAgents = new float[12][];

        string[] childAgentsString = new string[12];


        //72 / 12 = 6 for each .Each of those should be mutated. for each 6 indexes 1 of those parts should be mutated
        childAgentsString = GeneticCrossover(inputParents, alleleDivision);


        childAgentsString = Mutate(childAgentsString, alleleDivision);

        for (int i = 0; i < childAgents.Length; i++)
        {
            childAgents[i] = Array.ConvertAll(childAgentsString[i].Split(';'), float.Parse);
        }
        return childAgents; 

        
    }

    private string[] GeneticCrossover(float[][] inputParents,int alleleDivision)
    {
        float[][] crossoverAgents = new float[12][];
        string[] crossoverOutcome = new string[12];
        int arrayLength = inputParents[0].Length;

        crossoverAgents[0] = inputParents[0];
        crossoverAgents[1] = inputParents[1];

        int arrayCreateCounter = 2;

        for (int i = 0; i < inputParents.Length; i++)
        {
            for (int ii = i + 1; ii < inputParents.Length; ii++)
            {


                crossoverAgents[arrayCreateCounter] = inputParents[i];
                arrayCreateCounter++;
                crossoverAgents[arrayCreateCounter] = inputParents[ii];
                arrayCreateCounter++;


                for (int iii = 0; iii < arrayLength; iii += alleleDivision)
                {


                    if (UnityEngine.Random.value < 0.5)
                    {
                        float[] temp = new float[alleleDivision];

                        //Array.Copy(crossoverAgents[arrayCreateCounter - 2], iii, temp, 0, alleleDivision);
                        //Array.Copy(crossoverAgents[arrayCreateCounter - 1], iii, crossoverAgents[arrayCreateCounter - 2], iii, alleleDivision);
                        //Array.Copy(temp, 0, crossoverAgents[arrayCreateCounter - 1], iii, alleleDivision);




                        Swap(ref crossoverAgents[arrayCreateCounter - 1], iii, ref crossoverAgents[arrayCreateCounter - 2], iii, alleleDivision);

                    }

                }
                if (arrayCreateCounter >= 12)
                {
                    goto exit;
                }
                
            }
            
        }
        exit:

        for (int i = 0; i < crossoverOutcome.Length; i++)
        {
            crossoverOutcome[i] = string.Join(";", crossoverAgents[i]);
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
            geneticCodeToMutate[i] = Array.ConvertAll(inputNNCode[i].Split(';'), float.Parse);

        }





        for (int i = 0; i < geneticCodeToMutate.Length; i++)
        {
            for (int ii = 0; ii < geneticCodeToMutate[0].Length / alleleDivision; ii++)
            {
                geneticCodeToMutate[i][UnityEngine.Random.Range(alleleDivision * ii, (ii + 1) * 5)] = UnityEngine.Random.Range(-8f, 8f);
            }
        }
        

        for (int i = 0; i < geneticCodeToMutate.Length; i++)
        {
            mutationOutcome[i] = string.Join(";", geneticCodeToMutate[i]);

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

            firstBot.GetComponent<SNeuralNetwork>().NN.SetGeneticCode(geneticCodes[i - 1]);
            NNAgents[i - 1] = firstBot.GetComponent<SNeuralNetwork>();

            secondBot.GetComponent<SNeuralNetwork>().NN.SetGeneticCode(geneticCodes[i]);
            NNAgents[i] = secondBot.GetComponent<SNeuralNetwork>();

        }
    }


    void Swap(ref float[] one, int oneIndex, ref float[] two, int twoIndex, int length)
    {
        List<float> tempOne = new List<float>();
        List<float> tempTwo = new List<float>();

        for (int i = oneIndex; i < oneIndex + length; i++)
        {
            tempOne.Add(one[i]);
        }
        for (int i = twoIndex; i < twoIndex + length; i++)
        {
            tempTwo.Add(two[i]);
        }
        int index = 0;
        for (int i = oneIndex; i < oneIndex + length; i++)
        {
            one[i] = tempTwo[index++];
        }
        index = 0;
        for (int i = twoIndex; i < twoIndex + length; i++)
        {
            two[i] = tempOne[index++];
        }
    }


}