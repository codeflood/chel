# Host API #

Chel provides full control of the execution environment to the host application.

    var runtime = new ChelRuntime()

    // Adding commands
        .WithCommand<MyCommand>()
        .WithCommandsFromAssembly("assembly.dll")
    
    // Allow file access
        .FileAccess("tmp", @"c:\temp", FileAccess.Allow)
    // The tmp name is used to access the c:\temp disk location

    // Add a file script provider
        .AddScriptFolder("scripts");

To start a new session, call the `NewSession` method on the session runtime.

    var session = runtime.NewSession();

To retrieve an existing session, use the `GetSession` method.

    var session = runtime.GetSession(42);
