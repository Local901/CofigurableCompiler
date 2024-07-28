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

        public IReadOnlyList<IIOType> Outputs { get; private set; }

        public IReadOnlyList<IIOType> Inputs { get; private set; }

        private FunctionT? _functionHandler;
        protected FunctionT FunctionHandler {
            get {
                if (_functionHandler == null)
                {
                    throw new Exception($"FunctionStep {Name}: Function has not been set.");
                }
                return _functionHandler;
            }
            set
            {
                _functionHandler = value;
            }
        }

        /// <summary>
        /// Create a FunctionCallStep when extending the class.
        /// 
        /// !!! Don't forget to set <see cref="FunctionHandler"/> with a function.
        /// </summary>
        /// <param name="name">Name of the step</param>
        /// <param name="optionalProperties">Parameter names that are optional.</param>
        protected FunctionCallStep(string name, string[]? optionalProperties)
        {
            Name = name;

            optionalProperties ??= new string[0];

            var type = typeof(FunctionT).GetMethod("Invoke");

            Inputs = type.GetParameters()
                .Select((property, index) => new IOType(property.Name ?? $"{index}", property.GetType(), optionalProperties.Contains(property.Name), null))
                .ToList();

            Outputs = new List<IIOType>()
            {
                new IOType("result", type.ReturnType, true, null)
            };
        }

        /// <summary>
        /// Create a FunctionCallStep when extending the class.
        /// 
        /// !!! Don't forget to set <see cref="FunctionHandler"/> with a function.
        /// </summary>
        /// <param name="name">Name of the step</param>
        protected FunctionCallStep(string name)
            : this(name, new string[0]) { }

        public FunctionCallStep(string name, FunctionT function, string[]? optionalProperties)
            : this(name, optionalProperties)
        {
            FunctionHandler = function;
        }

        public FunctionCallStep(string name, FunctionT function)
            : this(name, function, new string[0]) { }

        public async Task<StepValue[]> Run(RunOptions options, InputOptions input)
        {
            var func = input.Scope.PrepairFunction<object, FunctionT>(
                "Invoke",
                input.StepValues
                    .Where((v) => v != null)
                    .Cast<StepValue>()
                    .Select((inValue) => new KeyValuePair<string, object>(inValue.PropertyName, inValue.GetValueAs<object>()))
                    .ToArray()
            );
            var result = func(FunctionHandler);
            if (result is Task task)
            {
                task.Wait();
                var r = typeof(Task<object>).GetProperty("Result")?.GetValue(task);

                if (r == null) return new StepValue[0];
                return new StepValue[]
                {
                    StepValue.CreateInstance(input.Scope, r.GetType(), "result", r)
                };
            }
            if (result == null) return new StepValue[0];
            return new StepValue[]
            {
                StepValue.CreateInstance(input.Scope, result.GetType(), "result", result)
            };
        }
    }
}
