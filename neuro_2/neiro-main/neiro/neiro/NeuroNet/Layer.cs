using System;
using neiro.NeuroNet.Math;

namespace neiro.NeuroNet
{
    abstract class Layer
    {
        protected Neuron[] neurons;

        public int Count => neurons.Length;

        public double[] GetOutputs()
        {
            double[] result = new double[neurons.Length];
            for (int i = 0; i < neurons.Length; i++)
                result[i] = neurons[i].Output;
            return result;
        }

        public abstract void Compute(double[] inputs);
    }
}
