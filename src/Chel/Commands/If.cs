using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Types;

namespace Chel.Commands
{
    [Command("if")]
    [Description("Conditionally execute a script block.")]
    public class If : ICommand
    {
        /// <summary>
        /// The session to execute scripts in.
        /// </summary>
        protected ISession Session { get; }

        [NumberedParameter(1, "condition")]
        [Description("The condition which determines whether to execute the script block.")]
        [Required]
        public bool ShouldExecute { get; set;}

        [NumberedParameter(2, "script")]
        [Description("The script block to execute if the condition is true.")]
        [Required]
        public string ScriptBlock { get; set;}

        [NamedParameter("else", "script")]
        [Description("The script block to execute if the condition is false.")]
        public string ElseScriptBlock { get; set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="session">The session to execute scripts in.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="session"/> is null.</exception>
        public If(ISession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public CommandResult Execute()
        {
            if(ShouldExecute)
            {
                if(!string.IsNullOrWhiteSpace(ScriptBlock))
                    Session.Execute(ScriptBlock);
            }
            else if(!string.IsNullOrWhiteSpace(ElseScriptBlock))
                Session.Execute(ElseScriptBlock);

            return SuccessResult.Instance;
        }
    }
}