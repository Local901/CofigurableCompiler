using ConLine.Options;
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

        public async Task<StepValue[]> Run(RunOptions options, InputOptions input)
        {
            var result = FunctionHandler.DynamicInvoke(Inputs.Select((inType) =>
                input.StepValues.FirstOrDefault((v) =>
                    v.PropertyName == inType.Name
                )?.GetValueAs<object?>() ?? inType.DefaultValue)
            );
            if (result is Task task)
            {
                task.Wait();
                var r = typeof(Task<object>).GetProperty("Result")?.GetValue(task);
                return new StepValue[]
                {
                    (StepValue)input.Scope.CreateInstance(typeof(StepValue<>).MakeGenericType(r.GetType()), new KeyValuePair<string, object>[]
                    {
                        new KeyValuePair<string, object>("propertyName", "result"),
                        new KeyValuePair<string, object>("value", r),
                    })
                };
            }
            return new StepValue[]
            {
                (StepValue)input.Scope.CreateInstance(typeof(StepValue<>).MakeGenericType(result.GetType()), new KeyValuePair<string, object>[]
                {
                    new KeyValuePair<string, object>("propertyName", "result"),
                    new KeyValuePair<string, object>("value", result),
                })
            };
        }
    }
}
