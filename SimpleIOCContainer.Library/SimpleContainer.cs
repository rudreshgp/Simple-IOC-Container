using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleIOCContainer.Library
{
    /// <summary>
    /// Simple IOC Container to do constructor dependency injection
    /// </summary>
    public sealed class SimpleContainer //: IDisposable
    {
        private SimpleContainer()
        {
        }

        /// <summary>
        /// Lock object is used while creating the singelton instance of the container
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// Container instance
        /// </summary>
        private static SimpleContainer _container;

        /// <summary>
        /// Dictionory holding all the registered dependencies
        /// </summary>
        private static readonly Dictionary<Type, Type> DepenedencyDictionary = new Dictionary<Type, Type>();

        /// <summary>
        /// Singelton instance object shared with the client
        /// </summary>
        public static SimpleContainer Container
        {
            get
            {
                if (_container == null)
                {
                    //Lock object so that other thread wont initialize the object
                    lock (LockObject)
                    {
                        if (_container == null)
                            _container = new SimpleContainer();
                    }
                }
                return _container;
            }
        }

        /// <summary>
        /// Register type TU as implementation for type T
        /// Used to register individual tightly coupled
        /// </summary>
        /// <typeparam name="T">interface type</typeparam>
        /// <typeparam name="TU">implementation type</typeparam>
        public void Register<T, TU>()
            where T : class
            where TU : T
        {
            if (!typeof(T).IsInterface)
                throw new InvalidOperationException($"{typeof(T).Name} is not an interface");
            if (!DepenedencyDictionary.ContainsKey(typeof(T)))
            {
                DepenedencyDictionary.Add(typeof(T), typeof(TU));
            }
        }

        /// <summary>
        /// register dependencies based on types
        /// </summary>
        /// <param name="interfaceType">interface type</param>
        /// <param name="implementationType">implementation type</param>
        public void Register(Type interfaceType, Type implementationType)
        {
            if (!interfaceType.IsInterface)
                throw new InvalidOperationException($"{interfaceType.Name} is not an interface");
            if (!DepenedencyDictionary.ContainsKey(interfaceType))
            {
                DepenedencyDictionary.Add(interfaceType, implementationType);
            }
        }

        /// <summary>
        /// Register all classes in the assembly based on the name
        /// </summary>
        /// <param name="assemblyName">full name of the assembly</param>
        public void RegisterAssembly(string assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            foreach (var type in assembly.GetTypes().Where(x => x.IsClass))
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    Register(interfaceType, type);
                }
            }
        }

        /// <summary>
        /// Gets an instance for type T
        /// </summary>
        /// <typeparam name="T">type of the interface</typeparam>
        /// <returns>instance of the implementation</returns>
        public T Resolve<T>() => (T)Resolve(typeof(T));

        /// <summary>
        /// Gets an instance for the interface type and returns as type T
        /// </summary>
        /// <typeparam name="T">return type or base type</typeparam>
        /// <param name="interfaceType">type to be resolved</param>
        /// <returns>instance of the implementation wrapped as T</returns>
        public T Resolve<T>(Type interfaceType) => (T)Resolve(interfaceType);
        
        /// <summary>
        /// Gets an instance for interface type but not tightly coupled
        /// </summary>
        /// <param name="interfaceType">interface type</param>
        /// <returns>instance of interface type</returns>
        public object Resolve(Type interfaceType)
        {
            Type implementation;
            if (!DepenedencyDictionary.TryGetValue(interfaceType, out implementation))
            {
                throw new InvalidOperationException($"Dependency for {interfaceType.Name} Not registered");
            }
            var constructorInfo = implementation.GetConstructors().SingleOrDefault();

            if (constructorInfo == null)
                throw new InvalidOperationException($"Constructor for {interfaceType.Name} Not Found");

            var parameterTypes = constructorInfo.GetParameters();

            var parameterInstances = parameterTypes.Select(
                                                    parameterInfo =>
                                                    Resolve(parameterInfo.ParameterType))
                                                    .ToArray();
            return constructorInfo.Invoke(parameterInstances);
        }

    }
}
