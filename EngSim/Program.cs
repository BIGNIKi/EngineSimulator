using EngSim.Src.ClassCreators;
using EngSim.Src.EngineTests;
using EngSim.Src.Interfaces;
using System;

namespace EngSim
{
    internal class Program
    {
        static void Main( string[] args )
        {
            var engine = EngineCreators.CreateCombustionEngine();

            engine.AmbientTemp = WaitForAmbientTempFromConsoleInput(); // °C температура среды
            engine.Accuracy = WaitForAccuracyConsoleInput(); // частота опроса симуляции двигателя

            // присоединение датчиков к двигателю для проведения тестов
            var test1 = AttachTheSensor1(engine); // датчик температуры
            var test2 = AttachTheSensor2(engine); // датчик мощности

            engine.Start();

            Console.WriteLine(test1.LogText);
            Console.WriteLine(test2.LogText);
        }

        private static double WaitForAmbientTempFromConsoleInput()
        {
            Console.Write("Введите значение температуры окружающей среды: ");
            double ambientTemp;
            while(!double.TryParse(Console.ReadLine(), out ambientTemp) || ambientTemp < -273.15)
            {
                if(ambientTemp < -273.15)
                {
                    Console.WriteLine("Температура по Цельсию не может быть ниже -273.15 °C.");
                }
                else
                {
                    Console.WriteLine("Введено некорректное значение!");
                }
                Console.Write("Введите значение температуры окружающей среды: ");
            }
            return ambientTemp;
        }

        private static double WaitForAccuracyConsoleInput()
        {
            Console.WriteLine("\nВведите значение частоты опроса симуляции двигателя.");
            Console.WriteLine("INFO - Допустимы следующие значения:");
            Console.WriteLine("INFO - (1 - каждую секунду), (10 - децисекунду), (100 - сантисекунду), (1000 - миллисекунду) и т.д.");
            double accuracy;
            while(!double.TryParse(Console.ReadLine(), out accuracy) || (accuracy != 1 && accuracy % 10 != 0) || accuracy <= 0)
            {
                Console.WriteLine("Введено некорректное значение!");
                Console.Write("Введите значение частоты опроса симуляции двигателя = ");
            }
            return accuracy;
        }

        private static EngineTestBase AttachTheSensor1( IEngine engine )
        {
            var test1 = new OverheatTimeTest(engine);
            test1.ConnectToEngine();
            return test1;
        }

        private static EngineTestBase AttachTheSensor2( IEngine engine )
        {
            var test2 = new MaxPowerTest(engine);
            test2.ConnectToEngine();
            return test2;
        }
    }
}
