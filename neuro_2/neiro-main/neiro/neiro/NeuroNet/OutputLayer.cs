using System;
using neiro.NeuroNet.Math;

namespace neiro.NeuroNet
{
    class OutputLayer : Layer
    {
        public OutputLayer(int neuronCount, int inputCount, Random rnd)
        {
            neurons = new Neuron[neuronCount];

            for (int i = 0; i < neuronCount; i++)
                neurons[i] = new Neuron(inputCount, rnd);
        }

        public override void Compute(double[] inputs)
        {
            foreach (var neuron in neurons)
            {
                neuron.SetInputs(inputs);
                neuron.ComputeOutput();
            }
        }
    }
}
