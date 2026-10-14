namespace neiro.NeuroNet
{
    enum MemoryMode//режим памяти
    {
        GET,
        SET,
        INIT
    }

    enum NeuronType//тип нейрона
    {
        Hidden,
        Output
    }

    enum NetworkMode//работа сети
    {
        Train,
        Test,
        Demo
    }
}