using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow
{
    class Program
    {
        static void Main(string[] args)
        {
            BlockingCollectionExample();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static void BlockingCollectionExample()
        {
            var builder = new CastingPipelineBuilder();

            //casting from object is needed on each step
            builder.AddStep(input => FindMostCommon(input as string));
            builder.AddStep(input => (input as string).Length);
            builder.AddStep(input => ((int)input) % 2 == 1);

            var pipeline = builder.GetPipeline();

            pipeline.Finished += res => Console.WriteLine(res);
            pipeline.Execute("The pipeline pattern is the best pattern");
            // 'True' is printed because 'pattern' is the most common with 7 chars and it's an odd number
            // ...
        }


        static string FindMostCommon(string input)
        {
            return input.Split(' ')
                .GroupBy(word => word)
                .OrderBy(group => group.Count())
                .Last()
                .Key;
        }
    }
}
