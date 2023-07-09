using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chel.Abstractions.Results
{
    /// <summary>
    /// A collection of <see cref="FailureResult" />.
    /// </summary>
    public class AggregateFailureResult : CommandResult
    {
        /// <summary>
        /// Gets the <see cref="FailureResult"s this instance contains.
        /// </summary>
        public IReadOnlyList<FailureResult> InnerResults { get; protected set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="innerResults">The <see cref="FailureResult"s this instance contains.</param>
        /// <exception cref="ArgumentNullException">If <paramref="innerResults" /> is null.</exception>
        public AggregateFailureResult(IList<FailureResult> innerResults)
        {
            InnerResults = new List<FailureResult>(innerResults ?? throw new ArgumentNullException(nameof(innerResults)));
            Success = false;
        }

        override public string ToString()
        {
            var buffer = new StringBuilder();
            
            foreach(var result in InnerResults)
            {
                buffer.AppendLine(result.ToString());
            }

            buffer.Remove(buffer.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            return buffer.ToString();
        }
    }
}
