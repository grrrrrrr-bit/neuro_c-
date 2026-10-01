using System;

namespace neiro.NeuroNet.Math
{
    class Neuron
    {
        private double[] weights;
        private double[] inputs;
        private double output;
        private double derivative;

        public Neuron(int inputCount, Random rnd)
        {
            weights = new double[inputCount];
            inputs = new double[inputCount];

            // Инициализация весов
            for (int i = 0; i < inputCount; i++)
                weights[i] = rnd.NextDouble() * 2 - 1; // [-1;1]
        }

        public void SetInputs(double[] inp)
        {
            inp.CopyTo(inputs, 0);
        }

        // Функция активации 
        private double Activate(double x)
        {
            return System.Math.Tanh(x);
        }

        private double ActivateDerivative(double x)
        {
            double t = System.Math.Tanh(x);
            return 1.0 - t * t;
        }


        // Вычисление выхода 
        public double ComputeOutput()
        {
            double sum = 0.0;

            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * inputs[i];

            output = Activate(sum);
            derivative = ActivateDerivative(sum);

            return output;
        }

        public double Output => output;
        public double Derivative => derivative;
        public double[] Weights => weights;
    }
}
