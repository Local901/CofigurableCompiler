using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Steps
{
    public class FunctionCallStep<FunctionT> : IStep where FunctionT : Delegate
    {
        public string Name { get; }

        public IReadOnlyList<IIOType> Outputs { get; }

        public IReadOnlyList<IIOType> Inputs { get; }

        private readonly FunctionT FunctionHandler;

        public FunctionCallStep(string name, FunctionT function) {
            Name = name;
            FunctionHandler = function;

            var type = typeof(FunctionT).GetMethod("Invoke");

            Inputs = type.GetParameters()
                .Select((property, index) => new IOType(property.Name ?? $"{index}", property.GetType(), false, null))
                .ToList();

            Outputs = new List<IIOType>()
            {
                new IOType("result", type.ReturnType, true, null)
            };
        }

        public async Task Run(RunOptions options, IStepInput input)
        {
            var result = FunctionHandler.DynamicInvoke(Inputs.Select((inType) =>
                input.Values.FirstOrDefault((v) =>
                    v.Name == inType.Name
                )?.Value ?? inType.DefaultValue)
            );
            if (result is Task task)
            {
                task.Wait();
                var r = typeof(Task<object>).GetProperty("Result")?.GetValue(task);
                input.SendResult("result", r);
                return;
            }
            input.SendResult("result", result);
        }
    }
}
