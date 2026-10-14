using System;
using System.IO;
using System.Windows.Forms;
using neiro;
using neiro.NeuroNet;

namespace neiro
{
    // Ссылок: 8
    abstract class Layer
    {
        // Поля
        // модификаторы protected стоят для внутрииерархического использования членов класса
        protected string name_Layer;        // намименование слоя, которое используется для
                                            // связи с одноимённым файлом памяти
        string pathDirWeights;              // путь к каталогу, где находится файл
                                            // синаптических весов для нейросети
        string pathFileWeights;             // путь к файлу синаптических весов для нейросети
        protected int numofneurons;         // число нейронов текущего слоя
        protected int numofprevneurons;     // число нейронов предыдущего слоя
        protected const double learningrate = 0.05d;  // скорость обучения
        protected const double momentum = 0.050d;     // момент инерции
        protected double[,] lastdeltaweights;         // веса предыдущей итерации обучения
        protected Neuron[] neurons;                   // массив нейронов текущего слоя

        // Свойства
        // Ссылок: 2
        public double[] Data // передача входных данных на нейроны слоя и активация нейронов
        {
            set
            {
                for (int i = 0; i < numofneurons; i++)
                {
                    neurons[i].Activator(value);
                }
            }
        }

        // Конструктор
        // Ссылок: 2
        protected Layer(int non, int nopn, NeuronType nt, string nm_Layer)
        {
            // nm_Layer используется для
            // связи с одноимённым полем слоя файлом памяти (увидите это в WeightInitialize)
            numofneurons = non;         // количество нейронов текущего слоя
            numofprevneurons = nopn;    // количество нейронов предыдущего слоя
            neurons = new Neuron[non];  // определение массива нейронов
            name_Layer = nm_Layer;      // наименование слоя, которое используется для связи
                                        // с одноимённым файлом памяти

            pathDirWeights = AppDomain.CurrentDomain.BaseDirectory + "memory\\"; // путь к каталогу,
                                                                                 // где находятся
                                                                                 // файлы синаптических
                                                                                 // весов для нейросети
            pathFileWeights = pathDirWeights + name_Layer + "_memory.csv";        // путь к файлу
                                                                                  // синаптических
                                                                                  // весов
                                                                                  // для текущего слоя

            lastdeltaweights = new double[non, nopn + 1];
            double[,] Weights; // временный массив синаптических весов текущего слоя

            if (File.Exists(pathFileWeights)) // определяет, существует ли pathFileWeights (файл)
                Weights = WeightInitialize(MemoryMode.GET, pathFileWeights); // считываем файл весов
            else
            {
                Directory.CreateDirectory(pathDirWeights);                   // Создание отсутствующей директории
                Weights = WeightInitialize(MemoryMode.INIT, pathFileWeights); // если файла весов нет
            }

            for (int i = 0; i < non; i++) // цикл формирования нейронов слоя и заполнения ими массива
            {
                double[] tmp_weights = new double[nopn + 1];
                for (int j = 0; j < nopn + 1; j++)
                {
                    tmp_weights[j] = Weights[i, j];
                }
                neurons[i] = new Neuron(tmp_weights, nt); // заполнение массива нейронами
            }
        }

        // Метод работы с массивом синаптических весов слоя
        // Ссылок: 5
        public double[,] WeightInitialize(MemoryMode mm, string path)
        {
            char[] delim = new char[] { ';' }; // Разделитель слов - ";" для чтения строк
            string tmpStr;                     // Временная строка
            string[] tmpStrWeights;            // Временный массив строк
            double[,] weights = new double[numofneurons, numofprevneurons + 1];

            switch (mm)
            {
                case MemoryMode.GET:
                    tmpStrWeights = File.ReadAllLines(path); // считывание строк текстового файла
                    string[] memory_elemnt;
                    for (int i = 0; i < numofneurons; i++)
                    {
                        memory_elemnt = tmpStrWeights[i].Split(delim); // разбивает строку
                        for (int j = 0; j < numofprevneurons + 1; j++)
                        {
                            // Replace нужен для замены запятой на точку, если в файле десятичный разделитель - запятая
                            weights[i, j] = double.Parse(memory_elemnt[j].Replace(',', '.'),
                                System.Globalization.CultureInfo.InvariantCulture); // преобразование строки в число
                        }
                    }
                    break;

                case MemoryMode.SET:
                    tmpStrWeights = new string[numofneurons]; // построчный массив (строковый формат) синаптические весов

                    if (!File.Exists(path)) // Если нет файла синаптических весов
                    {
                        MessageBox.Show("!!! В Н И М А Н И Е !!!\nФайл " + name_Layer + "_memory.csv\nНЕ НАЙДЕН!!!\nПосле нажатия кнопки ОК автоматически создастся отсутствующий файл, и все значения весов будут сохранены.",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    for (int i = 0; i < numofneurons; i++)
                    {
                        tmpStr = neurons[i].Weights[0].ToString();
                        for (int j = 1; j < numofprevneurons + 1; j++)
                        {
                            tmpStr += delim[0] + neurons[i].Weights[j].ToString();
                        }
                        tmpStrWeights[i] = tmpStr;
                    }

                    File.WriteAllLines(path, tmpStrWeights); // Создаёт новый файл, записывает в него указанный массив строк и затем закрывает его. Если целевой файл уже существует, он будет перезаписан.
                    break;

                case MemoryMode.INIT:
                    /* 
                     * ТРЕБОВАНИЕ:
                     * 1. Среднее значение весов = 0.
                     * 2. Количество нейронов (диапазон) = корень квадратный из предыдущего слоя.
                     * 
                     * В данном программном коде для каждого нейронного слоя имеется свой отдельный файл, 
                     * в котором хранятся значения синаптических весов нейронов этого слоя.
                     */

                    MessageBox.Show("!!! В Н И М А Н И Е !!!\nФайл " + name_Layer + "_memory.csv" + " НЕ НАЙДЕН!\n" +
                                    "Синаптические веса инициализируются случайным образом.\n" +
                                    "Нейросеть вернется к исходному состоянию «необученности», и потребуется обучать её ЗАНОВО.",
                                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    Random random = new Random(); // генератор случайных чисел

                    // 1. Вычисляем корень квадратный из предыдущего слоя (как вы просили)
                    double sqrtPrev = Math.Sqrt(numofprevneurons);

                    // 2. Задаем границу диапазона. Чтобы среднее было 0, диапазон должен быть симметричным: [-limit, +limit]
                    double limit = sqrtPrev;

                    double[] tmpArr = new double[numofprevneurons + 1]; // массив весов одного нейрона
                    tmpStrWeights = new string[numofneurons];          // массив строк синаптических весов

                    for (int i = 0; i < numofneurons; i++) // цикл перебора нейронов в слое
                    {
                        for (int j = 0; j < numofprevneurons + 1; j++)
                        {
                            // Генерируем случайное число от -limit до +limit.
                            // Формула: (random * 2 - 1) * limit дает диапазон [-limit, limit].
                            // Благодаря симметрии, среднее значение всех весов будет ровно 0.
                            tmpArr[j] = (random.NextDouble() * 2.0 - 1.0) * limit;
                        }

                        // Формирование строки для записи (преобразование массива чисел в строку через ';')
                        string line = string.Join(";", tmpArr);
                        tmpStrWeights[i] = line;
                    }

                    File.WriteAllLines(path, tmpStrWeights); // Сохраняем сгенерированные веса в файл

                    // Перечитываем только что созданный файл, чтобы вернуть корректный массив weights
                    tmpStrWeights = File.ReadAllLines(path);
                    for (int i = 0; i < numofneurons; i++)
                    {
                        string[] parts = tmpStrWeights[i].Split(delim);
                        for (int j = 0; j < numofprevneurons + 1; j++)
                        {
                            weights[i, j] = double.Parse(parts[j].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    break;

                default:
                    break;
            }

            return weights; // Возвращаем массив весов
        }
    }
}