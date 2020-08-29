using System;
using System.Collections;
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

    public class DataFlow<TIn, TOut>
    {
        public bool Debug { get; set; } = false;

        private ExecutionDataflowBlockOptions Options { get; }

        private List<IDataflowBlock> Blocks { get; } = new List<IDataflowBlock>();

        private bool Created { get; set; } = false;

        public DataFlow() => Options = new ExecutionDataflowBlockOptions();

        public DataFlow(ExecutionDataflowBlockOptions options) => Options = options;

        public DataFlow<TIn, TOut> Add<TLocalIn, TLocalOut>(Func<TLocalIn, TLocalOut> stepFunc)
        {
            var step = new TransformBlock<TC<TLocalIn, TOut>, TC<TLocalOut, TOut>>((tc) =>
            {
                if(Debug)
                    DebugStep(tc.Input);

                try
                {
                    return new TC<TLocalOut, TOut>(stepFunc(tc.Input), tc.TaskCompletionSource);
                }
                catch (Exception e)
                {
                    tc.TaskCompletionSource.SetException(e);
                    return new TC<TLocalOut, TOut>(default, tc.TaskCompletionSource);
                }
            }, Options);

            if (Blocks.Count > 0)
            {
                var lastStep = Blocks.Last();
                var targetBlock = (lastStep as ISourceBlock<TC<TLocalIn, TOut>>);
                targetBlock.LinkTo(step, new DataflowLinkOptions(),
                    tc => !tc.TaskCompletionSource.Task.IsFaulted);
                targetBlock.LinkTo(DataflowBlock.NullTarget<TC<TLocalIn, TOut>>(), new DataflowLinkOptions(),
                    tc => tc.TaskCompletionSource.Task.IsFaulted);
            }
            Blocks.Add(step);
            return this;
        }

        private void Create()
        {
            if (Created)
                throw new InvalidOperationException("Create was called on a DataFlow that is already created");
            
            var setResultStep =
                new ActionBlock<TC<TOut, TOut>>((tc) => tc.TaskCompletionSource.SetResult(tc.Input));
            var lastStep = Blocks.Last();
            var setResultBlock = (lastStep as ISourceBlock<TC<TOut, TOut>>);
            setResultBlock.LinkTo(setResultStep);
            Created = true;
        }

        public Task<TOut> Execute(TIn input)
        {
            if (!Created)
                Create();
            var firstStep = Blocks[0] as ITargetBlock<TC<TIn, TOut>>;
            var tcs = new TaskCompletionSource<TOut>();
            firstStep.SendAsync(new TC<TIn, TOut>(input, tcs));
            return tcs.Task;
        }

        private void DebugStep(object input)
        {
            var msg = $"IN: {input.GetType().Name}";

            if (input is ICollection collection)
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
