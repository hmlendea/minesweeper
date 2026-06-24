using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Minesweeper.Gui.Helpers
{
    public class FramerateCounter
    {
        static volatile FramerateCounter instance;
        static readonly Lock syncRoot = new();

        readonly Queue<float> sampleBuffer;

        public static FramerateCounter Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (syncRoot)
                    {
                        instance ??= new FramerateCounter();
                    }
                }

                return instance;
            }
        }

        public long TotalFrames { get; private set; }

        public float TotalSeconds { get; private set; }

        public float AverageFramesPerSecond { get; private set; }

        public float CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        public FramerateCounter() => sampleBuffer = new Queue<float>();

        public void Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                sampleBuffer.Dequeue();
                AverageFramesPerSecond = sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames += 1;
            TotalSeconds += deltaTime;
        }
    }
}
