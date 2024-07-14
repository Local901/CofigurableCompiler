using ConDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine.Options
{
    public class CompleteInputOptions : InputOptions, OutputOptions, MemoryOptions
    {
        public StepValue[] StepValues { get; }
        public Scope Scope { get; }

        private StepValue? OutputField;
        private MemorisedValue? MemoryField;

        public CompleteInputOptions(Scope scope, StepValue[] values, StepValue? output, MemorisedValue? memory)
        {
            Scope = scope;
            StepValues = values;
            OutputField = output;
            MemoryField = memory;
        }

        public void AddOuputValue<TType>(IEnumerable<TType> values)
        {
            if (!OutputField.IsArray)
            {
                throw new Exception("Can't assign an array to a single value.");
            }
            var enumerableOutput = OutputField.GetAs<IEnumerable<TType>>();
            if (enumerableOutput == null)
            {
                throw new Exception($"Values are of incorrect type. Was {nameof(IEnumerable<TType>)} but expected {OutputField.Type.FullName}");
            }
            var list = (enumerableOutput.Value ?? Enumerable.Empty<TType>()).ToList();
            list.AddRange(values);
            enumerableOutput.Value = list;
        }

        public void AddOutputValue<TType>(TType value)
        {
            if (OutputField.IsArray)
            {
                var enumerableOutput = OutputField.GetAs<IEnumerable<TType>>();
                if (enumerableOutput == null) {
                    throw new Exception("Failed to add to array.");
                }
                enumerableOutput.Value = enumerableOutput.Value.Append(value);
                return;
            }
            var outputField = OutputField.GetAs<TType>();
            if (outputField == null)
            {
                throw new Exception($"Output value not of corresponding type. Was {nameof(TType)} but expected {outputField.Type.FullName}");
            }
            outputField.Value = value;
        }

        public void SetMemory<TType>(TType value)
        {
            if (MemoryField == null)
            {
                throw new Exception("No memory field assigned.");
            }
            MemorisedValue memoryField = (MemorisedValue)MemoryField;
            memoryField.Value = value;
        }

        public TType? GetMemory<TType>()
        {
            return (TType?)MemoryField?.Value;
        }
    }
}
