using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace StudioLE.DataFlow
{
    public class TC<TInput, TOutput>
    {
        public TC(TInput input, TaskCompletionSource<TOutput> tcs)
        {
            Input = input;
            TaskCompletionSource = tcs;
        }
        public TInput Input { get; set; }
        public TaskCompletionSource<TOutput> TaskCompletionSource { get; set; }
    }

    public class Flow<TIn, TOut>
    {
        private List<IDataflowBlock> _transformBlocks = new List<IDataflowBlock>();
        public Flow<TIn, TOut> AddStep<TLocalIn, TLocalOut>(Func<TLocalIn, TLocalOut> stepFunc)
        {
            var step = new TransformBlock<TC<TLocalIn, TOut>, TC<TLocalOut, TOut>>((tc) =>
            {
                DebugStep(tc.Input);

                try
                {
                    return new TC<TLocalOut, TOut>(stepFunc(tc.Input), tc.TaskCompletionSource);
                }
                catch (Exception e)
                {
                    tc.TaskCompletionSource.SetException(e);
                    return new TC<TLocalOut, TOut>(default(TLocalOut), tc.TaskCompletionSource);
                }
            });

            if (_transformBlocks.Count > 0)
            {
                var lastStep = _transformBlocks.Last();
                var targetBlock = (lastStep as ISourceBlock<TC<TLocalIn, TOut>>);
                targetBlock.LinkTo(step, new DataflowLinkOptions(),
                    tc => !tc.TaskCompletionSource.Task.IsFaulted);
                targetBlock.LinkTo(DataflowBlock.NullTarget<TC<TLocalIn, TOut>>(), new DataflowLinkOptions(),
                    tc => tc.TaskCompletionSource.Task.IsFaulted);
            }
            _transformBlocks.Add(step);
            return this;
        }

        public Flow<TIn, TOut> CreateFlow()
        {
            var setResultStep =
                new ActionBlock<TC<TOut, TOut>>((tc) => tc.TaskCompletionSource.SetResult(tc.Input));
            var lastStep = _transformBlocks.Last();
            var setResultBlock = (lastStep as ISourceBlock<TC<TOut, TOut>>);
            setResultBlock.LinkTo(setResultStep);
            return this;
        }

        public Task<TOut> Execute(TIn input)
        {
            var firstStep = _transformBlocks[0] as ITargetBlock<TC<TIn, TOut>>;
            var tcs = new TaskCompletionSource<TOut>();
            firstStep.SendAsync(new TC<TIn, TOut>(input, tcs));
            return tcs.Task;
        }

        private void DebugStep(object input)
        {
            var msg = $"IN: {input.GetType().Name}";

            System.Collections.ICollection collection = input as System.Collections.ICollection;
            if (collection != null)
            {
                var colType = collection.GetType().GetGenericArguments()[0];
                msg += $"<{colType}>({collection.Count})";
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
