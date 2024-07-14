using ConLine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Steps
{
    public class Memory : IStep
    {
        public string Name { get; }

        public IReadOnlyList<IIOType> Outputs { get; }

        public IReadOnlyList<IIOType> Inputs { get; }

        public Memory(string name, Type type)
        {
            Name = name;
            Inputs = new IIOType[] { new IOType("value", type, false, null) };
            Outputs = new IIOType[] { new IOType("value", type, false, null) };
        }

        public Task<StepValue[]> Run(RunOptions options, InputOptions input)
        {
            if (input is MemoryOptions memoryOptions)
            {
                MemorizeValue(memoryOptions);
            }
            return Task.FromResult(input.StepValues);
        }

        public void MemorizeValue(MemoryOptions input)
        {
            var value = input.StepValues.FirstOrDefault((v) => v.PropertyName == Inputs[0].Name);
            if (value == null)
            {
                throw new Exception($"No input was provided to be memorised in {Name}");
            }
            typeof(MemoryOptions).GetMethod("SetMemory")
                ?.MakeGenericMethod(value.Type)
                .Invoke(input, new object?[] { value.GetValueAs<object>() });
        }
    }
}
