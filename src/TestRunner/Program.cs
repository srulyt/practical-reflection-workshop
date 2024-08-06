using System.Reflection;

Console.WriteLine("Practical Reflection Workshop Test Runner");
var path = new FileInfo(Environment.GetCommandLineArgs()[1]);
Console.WriteLine("Loading tests from " + path.FullName);
var testsDirectory = path.Directory;
System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += (r, a) => {
    var dllFilePath = testsDirectory.GetFiles(a.Name + ".dll").SingleOrDefault();
    if(dllFilePath != null) {
        return r.LoadFromAssemblyPath(dllFilePath.FullName);
    }
    return null;
};

var testAssembly = Assembly.LoadFile(path.FullName);
var testAssemblyTypes = testAssembly.GetTypes();
var testClasses = testAssemblyTypes.Where(t => t.IsClass && t.CustomAttributes.Any(a => a.AttributeType.Name == "TestClassAttribute"));

var testResults = new List<(string testName, Exception exception)>();
foreach(var testClass in testClasses) {
    var testClassIntance = Activator.CreateInstance(testClass);
    foreach(var testMethod in testClass.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.CustomAttributes.Any(a => a.AttributeType.Name == "TestMethodAttribute")))
    {
        try{
            testMethod.Invoke(testClassIntance, Array.Empty<object>());
            testResults.Add(new(testClass.Name + "." + testMethod.Name, null));
        }
        catch(Exception e){
            testResults.Add(new(testClass.Name + "." + testMethod.Name, e));
        }
    }
}

Console.WriteLine($"Completed running {testResults.Count} tests");
foreach(var result in testResults) {
    if(result.exception == null){
        Console.WriteLine("PASSED: " + result.testName);
    }
    else{
        Console.WriteLine("FAILED: " + result.testName);
        Console.WriteLine(result.exception);
    }
}