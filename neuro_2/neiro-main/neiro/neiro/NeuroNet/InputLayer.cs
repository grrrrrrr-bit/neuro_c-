using System;

namespace neiro.NeuroNet
{
    class InputLayer : Layer
    {
        private double[] outputs;

        public InputLayer(int count)
        {
            outputs = new double[count];
        }

        public override void Compute(double[] inputs)
        {
            inputs.CopyTo(outputs, 0);
        }

        public double[] GetOutputs() => outputs;
    }
}
