using System;
using System.Collections.Generic;
using System.Linq;
using AstroneurologicNet.DataLayer;

namespace AstroneurologicNet {
    internal static class Program {
        public static void Main(string[] args) {
            const int       totalSamples = 1000;
            const double    learningRate = .01;

            #region Sample Data
            var seed = new Random();
            var sightDataSet = (
                from i in Enumerable.Range(0, totalSamples)
                let color = new SightData(seed).ColorData
                select new {
                    red           = color.Red,
                    green         = color.Green,
                    blue          = color.Blue,
                    desiredOutput = color.Red > 126 && color.Green < 126 && color.Blue < 126 ? 1 : 0
                }).ToArray();
            #endregion

            const int trainingCount = totalSamples * 8 / 10; // 80% Training
            var       trainingSet   = sightDataSet.Take(trainingCount);
            var       testingSet    = sightDataSet.Skip(trainingCount);

            var sleepingNeuron = new SleepingNeuron();
            var wakingNeuron   = new WakingNeuron(sleepingNeuron);

            // Train
            foreach (var sample in trainingSet)
                wakingNeuron.Learn(sample.red, sample.green, sample.blue, sample.desiredOutput, learningRate);

            // Test
            var testResults = (
                    from sample in testingSet
                    let actualOutput = wakingNeuron.Wake(sample.red, sample.green, sample.blue)
                    let success = (actualOutput >= .5) == (sample.desiredOutput == 1)
                    let error = actualOutput - sample.desiredOutput
                    group error by success
            ).ToArray();
            
            // Log
            foreach (var sample in sightDataSet) {
                Console.Write(
                    "Sample Data \n {0} \n\n",
                    sample);
            }
            
            Console.Write(
                "Final State of Neuron \n Weight1: {0} \n Weight2: {1} \n Bias: {2} \n\n",
                sleepingNeuron.Weight1, sleepingNeuron.Weight2, sleepingNeuron.Bias);

            var queryResults1 = testResults
                .Select(record => new {Successful = record.Key, Count = record.Count()});
            foreach (var result in queryResults1) {
                Console.Write(
                    "Test Results - Summary \n {0} \n\n",
                    result);
            }

            Console.Write(
                "Average Error Magnitude \n {0} \n\n",
                testResults
                    .SelectMany(t => t)
                    .Average(Math.Abs));
            
            var queryResults2 = testResults
                .SelectMany(t => t)
                .GroupBy(loss => Math.Round(loss, 2))
                .Select(g => new {Error = g.Key, Count = g.Count()})
                .OrderBy(g => g.Error);
            foreach (var result in queryResults2) {
                Console.Write(
                    "Error Histogram \n {0} \n\n",
                    result);
            }
        }

        private class SleepingNeuron {
            public double Weight1, Weight2, Weight3;
            public double Bias;

            public SleepingNeuron() {
                Weight1 = GetSmallRandomNumber();
                Weight2 = GetSmallRandomNumber();
                Weight3 = GetSmallRandomNumber();
                Bias    = GetSmallRandomNumber();
            }

            private static readonly Random Random = new Random();

            private static double GetSmallRandomNumber() =>
                (.0009 * Random.NextDouble() + .0001) * (Random.Next(2) == 0 ? -1 : 1);
        }

        private class WakingNeuron {
            private readonly SleepingNeuron _neuron;
            private          double _totalInput;
            private          double _output;

            public WakingNeuron(SleepingNeuron neuron) => _neuron = neuron;

            public double Wake(double red, double green, double blue) {
                _totalInput =
                    red * _neuron.Weight1 +
                    green * _neuron.Weight2 +
                    blue * _neuron.Weight3 +
                    _neuron.Bias;

                return _output = _totalInput >= 0 ? _totalInput : _totalInput / 100;
            }

            public void Learn(double red, double green, double blue, double expectedOutput, double learningRate) {
                Wake(red, green, blue);
                
                // The loss is (Output - expectedOutput) * (Output - expectedOutput) / 2
                // When we derive this we get (Output - expectedOutput). We reverse the sign
                // because a positive gradient means we should move left and vice versa.
                var outputVotes = expectedOutput - _output;
                
                // Apply the chain rule: multiply the slope by the ReLU function
                var slopeOfRelu = _totalInput >= 0 ? 1 : .01;
                var inputVotes  = outputVotes * slopeOfRelu;

                var adjustment = inputVotes * learningRate;

                _neuron.Bias    += adjustment;
                _neuron.Weight1 += adjustment * red;
                _neuron.Weight2 += adjustment * green;
                _neuron.Weight3 += adjustment * blue;
            }
        }
    }
}