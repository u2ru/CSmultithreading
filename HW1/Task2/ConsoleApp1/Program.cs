// See https://aka.ms/new-console-template for more information

using System.Reflection;

var classLibraryAssembly = Assembly.LoadFrom("../net9.0/ClassLibrary1.dll");

var types = classLibraryAssembly.GetTypes().FirstOrDefault(type => type.Name == "Class1");

if (types == null)
{
    Console.WriteLine("Type 'Class1' not found in the assembly.");
    return;
}

dynamic myObject = Activator.CreateInstance(types);
Type paramType = myObject.GetType();


Console.WriteLine("Public Fields ======");
foreach (MemberInfo memberInfo in paramType.GetFields())
{
    Console.WriteLine(memberInfo.Name);
}

Console.WriteLine("\nPublic Methods ======");
foreach (MemberInfo memberInfo in paramType.GetMethods())
{
    Console.WriteLine(memberInfo.Name);
}

Console.WriteLine("\nPublic Properties =======");
foreach (MemberInfo memberInfo in paramType.GetProperties())
{
    Console.WriteLine(memberInfo.Name);
}

Console.WriteLine("\nPrivate Fields ======");
foreach (MemberInfo memberInfo in paramType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
{
    Console.WriteLine(memberInfo.Name);
}

Console.WriteLine("\nPrivate Methods ======");
foreach (MemberInfo memberInfo in paramType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
{
    Console.WriteLine(memberInfo.Name);
}

Console.WriteLine("\nPrivate Properties =======");
foreach (MemberInfo memberInfo in paramType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
{
    Console.WriteLine(memberInfo.Name);
}
