using System;

namespace neiro.NeuroNet
{
    class Network
    {
        private InputLayer inputLayer;
        private HiddenLayer hidden1;
        private HiddenLayer hidden2;
        private OutputLayer outputLayer;

        private Random rnd = new Random();

        public Network()
        {
            inputLayer = new InputLayer(15);
            hidden1 = new HiddenLayer(71, 15, rnd);
            hidden2 = new HiddenLayer(34, 71, rnd);
            outputLayer = new OutputLayer(10, 34, rnd);
        }

        public double[] Compute(double[] inputs)
        {
            inputLayer.Compute(inputs);

            hidden1.Compute(inputLayer.GetOutputs());
            hidden2.Compute(hidden1.GetOutputs());
            outputLayer.Compute(hidden2.GetOutputs());

            return outputLayer.GetOutputs();
        }
    }
}
