using System;
using System.Reflection;

// 1. name of class
Console.Write("Class Name: ");
string? className = Console.ReadLine();

if (string.IsNullOrWhiteSpace(className))
{
    return;
}

// class type
Type? classType = Type.GetType(className);
if (classType == null)
{
    Console.WriteLine($"Cant find {className}");
    return;
}

Console.WriteLine($"Class found: {classType.FullName}\n");

// 2. class instance
object? instance = Activator.CreateInstance(classType);
if (instance == null)
{
    return;
}

// 3. method name of class
Console.Write("Method name ");
string? methodName = Console.ReadLine();

if (string.IsNullOrWhiteSpace(methodName))
{
    return;
}

// 4. arguments for method
Console.WriteLine("\nEnter arguments for method (comma separated):");
Console.WriteLine("(example : int=42, string=Hello, double=3.14)");
    
string? argsInput = Console.ReadLine();
object[] parsedArgs = ParseArguments(argsInput ?? string.Empty);

Console.WriteLine("Arguments parsed:");

foreach (var arg in parsedArgs)
{
    Console.WriteLine($" - {arg} ({arg.GetType().Name})");
    PropertyInfo? prop = classType.GetProperty(arg.ToString());
    if (prop != null)
    {
        prop.SetValue(instance, arg);
    }
}

// 5. run method with reflection
MethodInfo? method = classType.GetMethod(methodName);
method?.Invoke(instance, parsedArgs);

// parse arguments implementation
object[] ParseArguments(string input)
{
    string[] argPairs = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    object[] args = new object[argPairs.Length];

    for (int i = 0; i < argPairs.Length; i++)
    {
        string[] parts = argPairs[i].Trim().Split('=');
        if (parts.Length != 2)
        {
            Console.WriteLine($"Invalid argument format: {argPairs[i]}");
            continue;
        }

        string typeName = parts[0].Trim();
        string valueStr = parts[1].Trim();

        try
        {
            Type type = Type.GetType(typeName, throwOnError: true);
            args[i] = Convert.ChangeType(valueStr, type);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing argument '{argPairs[i]}': {ex.Message}");
        }
    }

    return args;
}