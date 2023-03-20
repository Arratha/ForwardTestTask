using System;
using System.Globalization;

using ForwardTestTask.Benches;
using ForwardTestTask.Engines.ICE;


namespace ForwardTestTask
{
    class Program
    {
        private const float AbsoluteZero = -273.15f;

        private static void Main()
        {
            TestBench testBench = new TestBench();

            ICEngine engineForTest = new ICEngine(GetEnivronmentTemperature(), new ICEngineConfig());

            testBench.SetEngine(engineForTest);
            testBench.StartOverheatTest(
                () => Console.WriteLine("No engine found"),
                (float value) => Console.WriteLine($"Engine overheated at {value} sec"),
                () => Console.WriteLine("Test aborted. The temperature has stabilized."));

            Console.ReadKey();
        }

        private static float GetEnivronmentTemperature()
        {
            float? result;

            Console.Write("Enter the ambient temperature: ");
            result = TryGetTemperature();

            while (result == null)
            {
                result = TryGetTemperature();
            }

            return (float)result;

            float? TryGetTemperature()
            {
                string input = Console.ReadLine();

                input = input.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                input = input.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                if (float.TryParse(input, out float temp))
                {
                    if (temp < AbsoluteZero)
                    {
                        Console.Write($"The temperature cannot be less than {AbsoluteZero}. Enter correct ambient temperature: ");
                        return null;
                    }

                    return temp;
                }

                Console.Write("The data is incorrect. Enter correct ambient temperature: ");
                return null;
            }
        }
    }
}