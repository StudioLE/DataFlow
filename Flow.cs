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

        int bufferIndex;

        public event Action<object> Finished;

        public void AddStep<In, Out>(Func<In, Out> stepFunc)
        {
            _steps.Add(objInput => 
                stepFunc.Invoke((In)(object)objInput));
        }

        void RunStep(Func<object, object> step)
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

            bufferIndex = 0;

            _steps.ForEach(RunStep);

            return this;
        }
    }
}
