using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace TestAssemblyContextCollection
{
    class CustomLoadContext : AssemblyLoadContext
    {
        public CustomLoadContext(FileInfo serviceAssemblyFile, bool isCollectible)
            : base(isCollectible)
        {
            if (serviceAssemblyFile == null) throw new ArgumentNullException(nameof(serviceAssemblyFile));
            Resolver = new AssemblyDependencyResolver(serviceAssemblyFile.FullName);
        }

        private Uri ServiceBaseLocation { get; }

        private AssemblyDependencyResolver Resolver { get; }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = Resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = Resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                 return LoadUnmanagedDllFromPath(libraryPath);
            }
            return IntPtr.Zero;
        }
    }
}
