using System;
using System.Collections.Generic;
using UnityEngine;

public class SNeuralNetwork : MonoBehaviour
{

    #region Variables
    internal BotController NNbotController ;
    internal NeuralNetwork NN;
    internal int fitness = 0;
    #endregion


    /// <summary>
    /// A Collection of activation functions for compressing values
    /// </summary>
    internal class ActivationFunctions
    {
        /// <summary>
        /// This function is used to get valuese from -1 to 1 this will only be used for the rotation output.
        /// </summary>
        ///         /// <param name="x">
        /// This is the input for the function (float)
        /// </param>
        public static float SoftSign(float x)
        {
            return x / (1 + Math.Abs(x));
        }

        /// <summary>
        /// This function is used to get valuese from 0 to 1 this will only be used for the rotation output.
        /// </summary>
        /// <param name="x">
        /// This is the input for the function (float)
        /// </param>
        public static float Sigmoid(float x)
        {
            return 1 / (1 + Mathf.Exp(-x));
        }
    }

    /// <summary>
    /// The class that contains everything necessary for a neuron.
    /// </summary>
    internal class Neuron
    {

        

        /// <summary>
        /// All of the inputs of the neuron aka connections to it
        /// </summary>
        internal float[] inputs;
        /// <summary>
        /// All of the weights of the inputs of the neuron aka connections to it
        /// </summary>
        internal float[] weights;

        /// <summary>
        /// The weight of the neuron itself
        /// </summary>
        internal float biasWeight;

        #region Constructors
        /// <summary>
        /// Standard initialization of the neuron it only randomizes the weights
        /// </summary>
        internal Neuron()
        {
            RandomizeWeights();
        }

        /// <summary>
        /// Neuron initilization with the size of it's arrays: inputs[] and weights[]
        /// </summary>
        /// <param name="connectionAmount">
        /// The size of the inputs[] and weights[] arrays.
        /// </param>
        internal Neuron(int connectionAmount)
        {

            inputs = new float[connectionAmount];
            weights = new float[connectionAmount];
            RandomizeWeights();

        }
        #endregion

        #region Outputs
        /// <summary>
        /// Returns the output of the neuron in a form of value between -1 and 1 (SoftSign) 
        /// </summary>
        internal float SoftSignOutput
        {
            get
            {
                float sum = 0; // sum variable 
                for (int i = 0; i <= inputs.Length - 1; i++)
                {
                    sum += inputs[i] * weights[i]; // we sum the weight * input of each connection to this neuron
                }

                return ActivationFunctions.SoftSign(sum + biasWeight); // now we add the bias weight and pass it into the activation Function.

            }
        }

        /// <summary>
        /// Returns the output of the neuron in a form of value between 0 and 1 (Sigmoid) 
        /// </summary>
        internal float SigmoidOutput
        {
            get
            {
                float sum = 0; // sum variable 
                for (int i = 0; i <= inputs.Length -1; i++)
                {
                    sum += inputs[i] * weights[i]; // we sum the weight * input of each connection to this neuron
                }
                return ActivationFunctions.Sigmoid(sum + biasWeight); // now we add the bias weight and pass it into the activation Function.

            }
            
        }
        #endregion 

        /// <summary>
        /// Used for initialization of the Neuron (randomizes the weights)
        /// </summary>
        internal void RandomizeWeights()
        {
            for (int i = 0; i <= weights.Length - 1; i++)   
            {

                weights[i] = GenRandomWeight();
            }
            

            biasWeight = GenRandomWeight();
        }

        /// <summary>
        /// This is a helper function which is used to generate a random float between -4 and 4.
        /// It is used in RandomizeWeights
        /// </summary>
        /// <returns></returns>
        private float GenRandomWeight()
        {
            return UnityEngine.Random.Range(-8f, 8f);
        }
    }

    /// <summary>
    /// The class that will be the neural network itself.
    /// </summary>
    internal class NeuralNetwork
    {
        #region Variables
        #region Variables For Fitness
        //These values will be used for measuring fitness of out neural network

        /// <summary>
        /// This will determine which generation we will use for crossover
        /// </summary>
        int fitness = 0;
        #endregion

        #region Variables for initialization of NN 
        /// <summary>
        /// This is used for reference of the bot Controller.
        /// </summary>
        internal BotController botController;

        //internal float[] inputArray = new float[3] {}

        /// <summary>
        /// the genetic code that contains weights of each neuron of the nural network
        /// </summary>
        private float[] _genticCode = new float[78];


        public string ReadGeneticCode { get { return string.Join(",", _genticCode); } }
        public float[] ReadFloatGeneticCode { get { return this._genticCode; } }


        /// <summary>
        /// New Neuron[6] will be assigned later as a first Hidden layer of 6 neurons
        /// </summary>
        internal Neuron[] hL1;

        /// <summary>
        /// New Neuron[4] will be assigned later as a first Hidden layer of 4 neurons
        /// </summary>
        internal Neuron[] hL2;

        /// <summary>
        /// Will be assigned later as a rotation Ouput neuron of the network
        /// </summary>
        internal Neuron rotationOutputNeuron;

        /// <summary>
        /// Will be assigned later as a dash Ouput neuron of the network
        /// </summary>
        internal Neuron dashOutputNeuron;

        /// <summary>
        /// Will be assigned later as a angle dash back Ouput neuron of the network
        /// </summary>
        internal Neuron dashBackOutputNeuron;

        /// <summary>
        /// Will be assigned later as a angle view Ouput neuron of the network
        /// </summary>
        internal Neuron angleViewOutputNeuron;




        #endregion
        #endregion

        /// <summary>
        /// Constructor of the neural network which initializes the Neuron layers(arrays) and their sizes e.t.c
        /// </summary>
        internal NeuralNetwork(BotController botctrl)
        {
            botController = botctrl;
            rotationOutputNeuron = new Neuron(4);
            dashOutputNeuron = new Neuron(4);
            dashBackOutputNeuron = new Neuron(4);
            angleViewOutputNeuron = new Neuron(4);
            hL1 = InitializeLayer(6, 4);
            hL2 = InitializeLayer(4, 6);
            InitializeGeneticCode();
        }

        internal NeuralNetwork(BotController botctrl,float[] geneticCode)
        {
            botController = botctrl;
            rotationOutputNeuron = new Neuron(4);
            dashOutputNeuron = new Neuron(4);
            dashBackOutputNeuron = new Neuron(4);
            angleViewOutputNeuron = new Neuron(4);
            hL1 = InitializeLayer(6, 4);
            hL2 = InitializeLayer(4, 6);
            SetGeneticCode(geneticCode);
        }



        /// <summary>
        /// This function is used to initialize the layer(array) of neurons
        /// It is used in the NN constructor.
        /// </summary>
        /// <param name="arraySize">
        /// The size of the layer(array) of neurons</param>
        /// <returns>The initialized layer(array)</returns>
        private Neuron[] InitializeLayer(int arraySize)
        {
            Neuron[] neuronArray = new Neuron[arraySize];

            for (int i = 0; i <= arraySize - 1; i++)
            {
                neuronArray[i] = new Neuron();
            }

            return neuronArray;
        }

        /// <summary>
        /// This function is used to initialize the layer(array) of neurons with specified input and weight size
        /// It is used in the NN constructor.
        /// </summary>
        /// <param name="arraySize">
        /// The size of the layer(array) of neurons</param>
        /// <param name="connectionAmount">
        /// This is the amount of connections that each neuron has in this layer(array) of neurons</param>
        /// <returns>The initialized layer(array)</returns>
        private Neuron[] InitializeLayer(int arraySize,int connectionAmount)
        {
            Neuron[] neuronArray = new Neuron[arraySize];

            for (int i = 0; i <= arraySize - 1; i++)
            {
                neuronArray[i] = new Neuron(connectionAmount);
            }

            return neuronArray;
        }

        private void UpdateFirstLayerInputs()
        {
            //this will be used to initialize Inputs
            foreach (Neuron neuron in hL1)
            {

                for (int i = 0; i <= neuron.inputs.Length - 1;i++)
                {
                    if (i == 0)
                    {
                        neuron.inputs[i] = botController.IsEnemyInView;
                    }
                    else if (i == 1)
                    {
                        neuron.inputs[i] =  botController.IsEnemyDashing;
                    }
                    else if (i == 2)
                    {
                        neuron.inputs[i] =  botController.ViewAngle;
                    }
                    else if (i == 3)
                    {
                        neuron.inputs[i] = botController.EnemyDistance;
                    }
                }
            }
        }

        private void UpdateHiddenLayer(Neuron[] inputNeuronLayer, Neuron[] toUpdateNeuronLayer)
        {
            //this will be used to initialize Inputs
            foreach (Neuron neuron in toUpdateNeuronLayer)
            {
                for (int i = 0; i <= neuron.inputs.Length - 1; i++)
                {
                    neuron.inputs[i] = inputNeuronLayer[i].SigmoidOutput;
                }
            }
        }

        private void UpdateOutputInputs(Neuron[] inputNeuronLayer)
        {

            for (int i = 0; i <= rotationOutputNeuron.inputs.Length - 1; i++)
            {
                rotationOutputNeuron.inputs[i] = inputNeuronLayer[i].SoftSignOutput;
            }

            for (int i = 0; i <= angleViewOutputNeuron.inputs.Length - 1; i++)
            {
                angleViewOutputNeuron.inputs[i] = inputNeuronLayer[i].SoftSignOutput;
            }

            for (int i = 0; i <= dashOutputNeuron.inputs.Length - 1; i++)
            {
                dashOutputNeuron.inputs[i] = inputNeuronLayer[i].SigmoidOutput;
            }

            for (int i = 0; i <= dashBackOutputNeuron.inputs.Length - 1; i++)
            {
                dashOutputNeuron.inputs[i] = inputNeuronLayer[i].SigmoidOutput;
            }


            
        }


        public void GetOutputs()
        {
            UpdateFirstLayerInputs();
            UpdateHiddenLayer(hL1, hL2);
            UpdateOutputInputs(hL2);

            //Debug.Log(rotationOutputNeuron.SoftSignOutput);
            //Debug.Log(rotationOutputNeuron.SoftSignOutput);
            //Debug.Log(dashOutputNeuron.SigmoidOutput);

            botController.RotationButton = rotationOutputNeuron.SoftSignOutput;
            botController.ViewAngleChangeButton = angleViewOutputNeuron.SoftSignOutput;
            botController.DashButton = Convert.ToBoolean(Mathf.RoundToInt(dashOutputNeuron.SigmoidOutput));
        }

        internal int UpdateFitness()
        {
            if (botController.HasDealtDamage)
            {
                fitness += 10;
                botController.HasDealtDamage = false;
            }
            if (botController.HasDestroyedOpponent)
            {
                fitness += 30;
                botController.HasDestroyedOpponent = false;
            }
            if (botController.HasRecievedDamage)
            {
                fitness -= 10;
                botController.HasRecievedDamage = false;
            }
            if (botController.HasBeenDestroyed)
            {
                fitness -= 30;
                botController.HasBeenDestroyed = false;
            }


            return fitness;
        }

        public void InitializeGeneticCode()
        {
            List<float> tempList = new List<float>();

            for (int i = 0; i < hL1.Length; i++)
            {
                for (int ii = 0; ii < hL1[i].weights.Length; ii++)
                {
                    tempList.Add(hL1[i].weights[ii]);
                }
                tempList.Add(hL1[i].biasWeight);

            }

            for (int i = 0; i < hL2.Length; i++)
            {

                for (int ii = 0; ii < hL2[i].weights.Length; ii++)
                {
                    tempList.Add(hL2[i].weights[ii]);
                }
                tempList.Add(hL2[i].biasWeight);
            }

            Neuron[] outputLayer = { rotationOutputNeuron, dashOutputNeuron,dashBackOutputNeuron, angleViewOutputNeuron };
            for (int i = 0; i < outputLayer.Length; i++)
            {

                for (int ii = 0; ii < outputLayer[i].weights.Length; ii++)
                {
                    tempList.Add(outputLayer[i].weights[ii]);
                }
                tempList.Add(outputLayer[i].biasWeight);
            }


            _genticCode = tempList.ToArray();
        }

        public void SetGeneticCode(string inputGeneticCode)
        {
            _genticCode = Array.ConvertAll(inputGeneticCode.Split(','), float.Parse);
            ApplyGeneticCode();
        }

        public void SetGeneticCode(float[] inputGeneticCode)
        {
            _genticCode = inputGeneticCode;
            ApplyGeneticCode();
        }

        private void ApplyGeneticCode()
        {
            float[] tempArray = _genticCode;
            Queue<float> tempQueue = new Queue<float>(tempArray);
            
            
            for (int i = 0; i < hL1.Length; i++)
            {
                for (int ii = 0; ii < hL1[i].weights.Length; ii++)
                {
                    hL1[i].weights[ii] = tempQueue.Dequeue();
                }
                hL1[i].biasWeight = tempQueue.Dequeue();

            }

            for (int i = 0; i < hL2.Length; i++)
            {

                for (int ii = 0; ii < hL2[i].weights.Length; ii++)
                {
                    hL2[i].weights[ii] = tempQueue.Dequeue();
                }
                hL2[i].biasWeight = tempQueue.Dequeue();
            }

            Neuron[] outputLayer = { rotationOutputNeuron, dashOutputNeuron, dashBackOutputNeuron, angleViewOutputNeuron };
            for (int i = 0; i < outputLayer.Length; i++)
            {

                for (int ii = 0; ii < outputLayer[i].weights.Length; ii++)
                {
                    outputLayer[i].weights[ii] = tempQueue.Dequeue();
                }
                outputLayer[i].biasWeight = tempQueue.Dequeue();
            }
        }

    }


    private void Awake()
    {

        NNbotController = GetComponent<BotController>();
        NN = new NeuralNetwork(NNbotController);
    }

    private void Update()
    {
        NN.GetOutputs();
        fitness = NN.UpdateFitness();


        //NN.SetGeneticCode("1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7");


    }

}