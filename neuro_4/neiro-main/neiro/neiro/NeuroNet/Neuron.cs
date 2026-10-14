using neiro.NeuroNet;
using static System.Math;

namespace neiro
{
    class Neuron
    {
        // поля
        private NeuronType type; // тип нейрона
        private double[] weights; // его веса
        private double[] inputs; // его входы
        private double output; // его выход
        private double derivative; // производная

        // Две константы
        private double a = 0.01d;
        private double b = 0.01d;

        // свойства
        public double[] Weights { get => weights; set => weights = value; }
        public double[] Inputs { get => inputs; set => inputs = value; }
        public double Output { get => output; }
        public double Derivative { get => derivative; }

        // конструктор
        public Neuron(double[] memoryWeights, NeuronType typeNeuron)
        {
            type = typeNeuron;
            weights = memoryWeights;
        }

        public void Activator(double[] i)
        {
            inputs = i; //передача вектора входного сигнала в массив входных данных нейрона

            double sum = weights[0]; //аффинное преобразование через смещение

            for (int j = 0; j < inputs.Length; j++)
            {
                sum += inputs[j] * weights[j + 1];
            }

            switch (type)
            {
                case NeuronType.Hidden:
                    output = HyperbolicTangent(sum);
                    derivative = HyperbolicTangentDerivative(sum);
                    break;

                case NeuronType.Output:
                    output = Exp(sum);
                    break;
            }
        }

        // --- Наша функция (продолжение после 53 строки) ---

        /// <summary>
        /// Гиперболический тангенс, вычисленный через экспоненту.
        /// Формула: (e^x - e^-x) / (e^x + e^-x)
        /// </summary>
        private double HyperbolicTangent(double x)
        {
            // Вычисляем экспоненту для x и -x
            double expX = Exp(x);
            double expMinusX = Exp(-x);

            // Возвращаем результат по формуле
            return (expX - expMinusX) / (expX + expMinusX);
        }

        /// <summary>
        /// Производная гиперболического тангенса.
        /// Формула: 1 - tanh^2(x)
        /// </summary>
        private double HyperbolicTangentDerivative(double x)
        {
            double tanh = HyperbolicTangent(x);
            return 1.0d - (tanh * tanh);
        }
    }
}