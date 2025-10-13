using UnityEngine;

public class ExampleDevCommands : MonoBehaviour
{
    [DevCommand("hello", "Replies with a greeting")] 
    public string Hello(string[] args)
    {
        return "Hello from ExampleDevCommands";
    }

    [DevCommand("echo", "Echo back provided text")] 
    public string Echo(string[] args)
    {
        return args == null || args.Length == 0 ? string.Empty : string.Join(" ", args);
    }
}


