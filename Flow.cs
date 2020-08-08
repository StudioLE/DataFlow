using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Flow
{
    public interface IFlow
    {
        void Execute(object input);
        event Action<object> Finished;
    }

    public class FlowBuilder : IFlow
    {
        List<Func<object, object>> _steps = new List<Func<object, object>>();

        BlockingCollection<object>[] _buffers;

        public event Action<object> Finished;

        public void AddStep(Func<object, object> stepFunc)
        {
            _steps.Add(stepFunc);
        }

        public void Execute(object input)
        {
            var first = _buffers[0];
            first.Add(input);
        }

        public IFlow Flow()
        {
            _buffers = _steps // Create buffers
                .Select(step => new BlockingCollection<object>())
                .ToArray();

            int bufferIndex = 0;
            foreach (var step in _steps)
            {
                var bufferIndexLocal = bufferIndex; // so it remains the same in each thread
                Task.Run(() =>
                {
                    // 'GetConsumingEnumerable' is blocking when the collection is empty
                    foreach (var input in _buffers[bufferIndexLocal].GetConsumingEnumerable())
                    {
                        var output = step.Invoke(input);

                        bool isLastStep = bufferIndexLocal == _steps.Count - 1;
                        if (isLastStep)
                        {
                            // This is dangerous as the invocation is added to the last step
                            // Alternatively, you can utilize 'BeginInvoke' like here: https://stackoverflow.com/a/16336361/1229063
                            Finished?.Invoke(output);
                        }
                        else
                        {
                            var next = _buffers[bufferIndexLocal + 1];
                            next.Add(output); // output will be stored as object
                        }
                    }
                });
                bufferIndex++;
            }
            return this;
        }
    }
}
