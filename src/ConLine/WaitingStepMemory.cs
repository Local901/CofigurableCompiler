using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConLine
{
    public class WaitingStepMemory
    {
        public readonly IStep Step;
        private StepValue[] inputValues;
        public IReadOnlyList<StepValue> InputValues { get => inputValues; }

        public WaitingStepMemory(IStep step)
        {
            Step = step;
            inputValues = new StepValue[step.Inputs.Count];
        }

        public void SetInputValue(StepValue inputValue)
        {
            for (int i = 0; i < Step.Inputs.Count; i++)
            {
                var input = Step.Inputs[i];
                if (input.Name == inputValue.PropertyName)
                {
                    inputValues[i] = inputValue;
                    return;
                }
            }
            throw new Exception($"No input with name: {inputValue.PropertyName}");
        }

        public bool IsInputValid()
        {
            var inputTypes = Step.Inputs;
            for (int i = 0; i < inputValues.Length; i++)
            {
                var value = inputValues[i];
                var inputType = inputTypes[i];
                if (inputType.IsOptional) continue;
                if (value == null) return false;
            }
            return true;
        }
    }
}
