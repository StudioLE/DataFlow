using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Geometry;

namespace Flow
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await BlockingCollectionExampleAsync();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static async Task BlockingCollectionExampleAsync()
        {
            var solids = new List<Solid>()
            {
                new Cuboid(1, 1, 1)  { Color = Color.Magenta }, // 0
                new Cuboid(1, 1, 2)  { Color = Color.Cyan },    // 1
                new Cuboid(1, 4, 2)  { Color = Color.Gold },    // 2
                new Cuboid(1, 1, 2)  { Color = Color.Green },   // 3
                new Cuboid(3, 3, 3)  { Color = Color.Fuchsia }, // 4
                new Cube(1)          { Color = Color.Blue },    // 5
                new Cube(2)          { Color = Color.Lime },    // 6
                new Cube(3)          { Color = Color.Purple },  // 7
                new Sphere(0.5)      { Color = Color.Orange },  // 8
                new Sphere(1)        { Color = Color.Yellow },  // 9
                new Sphere(2)        { Color = Color.Red },     // 10
                new Cylinder(0.5, 1) { Color = Color.Indigo },  // 11
                new Cylinder(3, 6)   { Color = Color.Lavender } // 12
            };

            var flow = CreateFlow();

            var tcs = new TaskCompletionSource<KnownColor>();

            var tc = new TC<List<Solid>, KnownColor>(solids, tcs);

            var task = tcs.Task;

            await flow.SendAsync(tc);

            var result = await task;

            Console.WriteLine(result);
            // After execution completes we should see "Fuchsia" printed to the console
        }

        public static TransformBlock<TC<List<Solid>, KnownColor>, TC<List<Cuboid>, KnownColor>> CreateFlow()
        {
            // Cuboids only
            var step1 = new TransformBlock<TC<List<Solid>, KnownColor>, TC<List<Cuboid>, KnownColor>>(
                tc => new TC<List<Cuboid>, KnownColor> (
                    CuboidFilter(tc.Input),
                    tc.TaskCompletionSource
                )
           );

            // Mass greater than 2
            var step2 = new TransformBlock<TC<List<Cuboid>, KnownColor>, TC<List<Cuboid>, KnownColor>>(
                tc => new TC<List<Cuboid>, KnownColor>(
                    tc.Input.Where(x => x.Mass > 2).ToList(),
                    tc.TaskCompletionSource
                )
            );

            // Colours
            var step3 = new TransformBlock<TC<List<Cuboid>, KnownColor>, TC<List<Color>, KnownColor>>(
                tc => new TC<List<Color>, KnownColor>(
                    tc.Input.Select(x => x.Color).ToList(),
                    tc.TaskCompletionSource
                )
            );

            // At least half red
            var step4 = new TransformBlock<TC<List<Color>, KnownColor>, TC<List<Color>, KnownColor>>(
                tc => new TC<List<Color>, KnownColor>(
                    tc.Input.Where(x => x.R >= 128).ToList(),
                    tc.TaskCompletionSource
                )
            );

            // Order by blue
            var step5 = new TransformBlock<TC<List<Color>, KnownColor>, TC<List<Color>, KnownColor>>(
                tc => new TC<List<Color>, KnownColor>(
                    tc.Input.OrderByDescending(x => x.B).ToList(),
                    tc.TaskCompletionSource
                )
            );

            // Highest blue
            var step6 = new TransformBlock<TC<List<Color>, KnownColor>, TC<Color, KnownColor>>(
                tc => new TC<Color, KnownColor>(
                    tc.Input.First(),
                    tc.TaskCompletionSource
                )
            );

            // Name of the colour
            var step7 = new TransformBlock<TC<Color, KnownColor>, TC<KnownColor, KnownColor>>(
                tc => new TC<KnownColor, KnownColor>(
                    tc.Input.ToKnownColor(),
                    tc.TaskCompletionSource
                )
            );

            var setResultStep = new ActionBlock<TC<KnownColor, KnownColor>>(tc => tc.TaskCompletionSource.SetResult(tc.Input));

            step1.LinkTo(step2, new DataflowLinkOptions());
            step2.LinkTo(step3, new DataflowLinkOptions());
            step3.LinkTo(step4, new DataflowLinkOptions());
            step4.LinkTo(step5, new DataflowLinkOptions());
            step5.LinkTo(step6, new DataflowLinkOptions());
            step6.LinkTo(step7, new DataflowLinkOptions());
            step7.LinkTo(setResultStep, new DataflowLinkOptions());
            return step1;
        }

        static List<Cuboid> CuboidFilter(List<Solid> input)
        {
            return input.Where(x => x is Cuboid)
                .Cast<Cuboid>()
                .ToList();
        }
    }
}
