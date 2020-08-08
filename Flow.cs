using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Flow
{
    public interface IFlowStep<TStepIn>
    {
        BlockingCollection<TStepIn> Buffer { get; set; }
    }

    public class FlowStep<TStepIn, TStepOut> : IFlowStep<TStepIn>
    {
        public BlockingCollection<TStepIn> Buffer { get; set; } = new BlockingCollection<TStepIn>();
        public Func<TStepIn, TStepOut> StepAction { get; set; }
    }

    public static class FlowExtensions
    {
        public static TOutput Step<TInput, TOutput, TInputOuter, TOutputOuter>
            (this TInput inputType,
            Flow<TInputOuter, TOutputOuter> builder,
            Func<TInput, TOutput> step)
        {
            var flowStep = builder.GenerateStep<TInput, TOutput>();
            flowStep.StepAction = step;
            return default(TOutput);
        }
    }

    public class Flow<TFlowIn, TFlowOut>
    {
        List<object> _steps = new List<object>();

        public event Action<TFlowOut> Finished;

        public Flow(Func<TFlowIn, Flow<TFlowIn, TFlowOut>, TFlowOut> steps)
        {
            steps.Invoke(default(TFlowIn), this);//Invoke just once to build blocking collections
        }

        public void Execute(TFlowIn input)
        {
            var first = _steps[0] as IFlowStep<TFlowIn>;
            first.Buffer.Add(input);
        }

        public FlowStep<TStepIn, TStepOut> GenerateStep<TStepIn, TStepOut>()
        {
            var flowStep = new FlowStep<TStepIn, TStepOut>();
            var stepIndex = _steps.Count;

            Task.Run(() =>
            {
                IFlowStep<TStepOut> nextFlowStep = null;

                foreach (var input in flowStep.Buffer.GetConsumingEnumerable())
                {
                    bool isLastStep = stepIndex == _steps.Count - 1;
                    var output = flowStep.StepAction(input);
                    if (isLastStep)
                    {
                        // This is dangerous as the invocation is added to the last step
                        // Alternatively, you can utilize BeginInvoke like here: https://stackoverflow.com/a/16336361/1229063
                        Finished?.Invoke((TFlowOut)(object)output);
                    }
                    else
                    {
                        nextFlowStep = nextFlowStep // no need to evaluate more than once
                            ?? (isLastStep ? null : _steps[stepIndex + 1] as IFlowStep<TStepOut>);
                        nextFlowStep.Buffer.Add(output);
                    }
                }
            });

            _steps.Add(flowStep);
            return flowStep;
        }
    }
}
