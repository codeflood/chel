using System;
using Chel.Abstractions;
using Chel.Abstractions.Results;
using Chel.Abstractions.Parsing;
using Chel.Abstractions.Variables;
using Chel.Commands;
using Chel.Parsing;
using System.Collections.Generic;

namespace Chel
{
    /// <summary>
    /// The Chel runtime and host API.
    /// </summary>
    public class Runtime
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly ICommandServices _commandServices;
        private readonly IParser _parser;
        private readonly IVariableReplacer _variableReplacer;
        private readonly IScopedObjectRegistry _sessionObjectTypes;
        private readonly List<IScriptProvider> _scriptProviders;
        private readonly ScriptProviderCollection _scriptProvidersCollection;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public Runtime()
        {
            var nameValidator = new NameValidator();
            var commandDescriptorGenerator = new CommandAttributeInspector();
            _commandRegistry = new CommandRegistry(nameValidator, commandDescriptorGenerator);

            _commandServices = new CommandServices();
            _parser = new Parser(nameValidator);
            _variableReplacer = new VariableReplacer();
            
            _sessionObjectTypes = new ScopedObjectRegistry();
            _sessionObjectTypes.Register<VariableCollection>();

            _scriptProviders = new List<IScriptProvider>();
            _scriptProvidersCollection = new ScriptProviderCollection(_scriptProviders);

            _commandRegistry.Register(typeof(Help));
            _commandRegistry.Register(typeof(Var));

            _commandServices.Register(_commandRegistry);
            _commandServices.Register<INameValidator>(nameValidator);
            _commandServices.Register<IExecutionTargetIdentifierParser>(new ExecutionTargetIdentifierParser());
            _commandServices.Register<IScriptProvider>(_scriptProvidersCollection);
        }

        /// <summary>
        /// Register a command type.
        /// </summary>
        /// <param name="commandType">The <see cref="Type" /> implementing the command.</param>
        public void RegisterCommandType(Type commandType)
        {
            if(commandType == null)
                throw new ArgumentNullException(nameof(commandType));

            _commandRegistry.Register(commandType);
        }

        /// <summary>
        /// Register a command service.
        /// </summary>
        /// <typeparam name="T">The type of the service being registered.</typeparam>
        /// <param name="service">The service instance to register.</param>
        public void RegisterCommandService<T>(T service)
        {
            if(service == null)
                throw new ArgumentNullException(nameof(service));

            _commandServices.Register<T>(service);
        }

        /// <summary>
        /// Register a session scoped service.
        /// </summary>
        /// <typeparam name="T">The type of the service being registered.</typeparam>
        public void RegisterSessionService<T>()
        {
            _sessionObjectTypes.Register<T>();
        }

        /// <summary>
        /// Register a new script provider.
        /// </summary>
        /// <param name="provider">The provider to register.</param>
        public void RegisterScriptProvider(IScriptProvider provider)
        {
            if(provider == null)
                throw new ArgumentNullException(nameof(provider));
            
            _scriptProviders.Add(provider);
        }

        /// <summary>
        /// Creates a new <see cref="ISession" />.
        /// </summary>
        public ISession NewSession(Action<CommandResult> resultHandler)
        {
            var sessionObjects = _sessionObjectTypes.CreateScope();
            var factory = new CommandFactory(_commandRegistry, _commandServices, sessionObjects);

            var variables = sessionObjects.Resolve(typeof(VariableCollection)) as VariableCollection;
            var parameterBinder = new CommandParameterBinder(_commandRegistry, _variableReplacer, variables ?? new VariableCollection());
            
            var session = new Session(_parser, factory, parameterBinder, _scriptProvidersCollection, resultHandler);
            sessionObjects.RegisterInstance<ISession>(session);

            return session;
        }
    }
}