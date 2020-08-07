# test-assembly-context-unloading

This projects demonstrates that even if there is a strong reference to an assembly of a collectible custom assembly load context in some other context (here 'default'), the assembly is getting garbage collected.

## Setup

1. Clone the repo
2. Navigate to ClassLibrary1 folder, run `dotnet publish` (debug mode).
3. Run `TestAssemblyContextCollection`.

This should throw an exception.

4. Now uncomment `Line#37` of Program.cs in `TestAssemblyContextCollection` project (https://github.com/sankalan/test-assembly-context-unloading/blob/develop/TestAssemblyContextCollection/Program.cs#L37).
5. Re-run the project, this time it should complete successfully.

## What's happening

If you go through the code in `Program.cs` of  `TestAssemblyContextCollection` project, you can see that all assemblies are loaded in different contexts. 
Target is to keep hold of few special assemblies depending on their some logic and unload the rest.
But for that it seems like we need to explicitly have reference to the desired `loadcontext`.
You can keep an eye on the `console` which logs when an assembly gets loaded/unloaded.
Also there are inline comments in `Program.cs` about different outcomes on trying `GC.KeepAlive` / `GC.SuppressFinalise`.

## Expectation

As per this documention- https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability#troubleshoot-unloadability-issues, it's expected that having reference to an assembly of the load context should keep the load context alive.
