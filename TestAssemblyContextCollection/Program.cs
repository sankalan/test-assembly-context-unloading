using Common;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading;

namespace TestAssemblyContextCollection
{
    class Program
    {
        static IEnumerable<Assembly> GetAssemblies()
        {
            var DllSearchDirectory = new DirectoryInfo("..\\..\\..\\..\\ClassLibrary1\\bin\\Debug\\netcoreapp3.1\\publish");
            string DllSearchPattern = "*.dll";
            foreach (var file in DllSearchDirectory.EnumerateFiles(DllSearchPattern, SearchOption.AllDirectories))
            {
                Console.WriteLine("\nLoading: " + file.Name);
                var loadContext = new CustomLoadContext(file, isCollectible: true);
                loadContext.Unloading += (a) =>
                {
                    Console.WriteLine("\nUnloading: " + file.Name);
                };
                var assembly = TryLoadAssembly(loadContext, file);
                if (assembly != null && IsSpecialAssembly(assembly))
                {
                    Console.WriteLine("\nFound special type in: " + file.Name);
                    // Keeping a reference to the loadContext explicitly here keeps safe from garbage collection
                    assembly.ModuleResolve +=
                        (sender, e) =>
                        {
                            //Console.WriteLine("\nSome log: " + assembly.GetTypes()); /* Keeping a reference to an assembly of that context doesn't help*/
                            //Console.WriteLine("\nSome log: " + loadContext.Name); /* Explicit reference to loadcontext */
                            return null;
                        };
                    //GC.KeepAlive(loadContext); /* Doesn't work */
                    //GC.SuppressFinalize(loadContext); /* Saves it from getting collected, but invocation through reflection fails */
                    yield return assembly;
                }
                else
                {
                    loadContext.Unload();
                }
            }
        }

        static bool IsSpecialAssembly(Assembly assembly)
        {
            return GetLoadableTypes(assembly).Any(type => type != null && type.GetCustomAttribute<SpecialAttribute>()!= null);
        }

        static Assembly TryLoadAssembly(AssemblyLoadContext loader, FileInfo file)
        {
            try
            {
                return loader.LoadFromAssemblyPath(file.FullName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static Type[] GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types;
            }
        }

        static void Main(string[] args)
        {
            List<Assembly> assemblies = GetAssemblies().ToList();
            GC.Collect();
            Thread.Sleep(1000);
            Console.WriteLine($"\nThere are {assemblies.Count} assemblies with Special types");
            
            Type special = GetLoadableTypes(assemblies[0])[0];
            dynamic instance = Activator.CreateInstance(special);
            instance.Do();

            Console.WriteLine("\n\nPress a key to exit...");
            Console.ReadKey();
        }
    }
}
