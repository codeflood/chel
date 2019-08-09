# Host API #

Chel provides full control of the execution environment to the host application.

    var factory = new ChelSessionFactory();

    // Adding commands
    factory.WithCommand()
    factory.WithCommands();
    
    // Allow file access
    factory.FileAccess("temp", FileAccess.Allow);

    // Add a file script provider
    factory.AddScriptFolder("scripts");

To start a new session, call the `NewSession` method on the session factory.

    var session = factory.NewSession();

To retrieve an existing session, use the `GetSession` method.

    var session = factory.GetSession();
