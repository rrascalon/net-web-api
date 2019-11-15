using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Net.Web.Api.Sdk.Injection.Attributes;

namespace Net.Web.Api.Sdk.Injection.Installers
{
    /// <inheritdoc />
    /// <summary>
    /// Class ServiceInstaller.
    /// </summary>
    /// <seealso cref="T:Castle.MicroKernel.Registration.IWindsorInstaller" />
    public class ServiceInstaller : IWindsorInstaller
    {
        #region Private Properties

        /// <summary>
        /// The assembly name prefix
        /// </summary>
        private string _assemblyNamePrefix;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInstaller"/> class.
        /// </summary>
        /// <param name="assemblyNamePrefix">The assembly name prefix.</param>
        public ServiceInstaller(string assemblyNamePrefix = null)
        {
            _assemblyNamePrefix = assemblyNamePrefix;
        }

        #endregion

        #region IWindsorInstaller Implementations

        /// <inheritdoc />
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var assemblyDescriptor = Classes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.RelativeSearchPath));

            if (assemblyDescriptor == null)
            {
                return;
            }

            var assemblyList = GetAssemblyList(assemblyDescriptor);
            var registrationList = GetRegistrationList(assemblyList);

            foreach(var item in registrationList)
            {
                container.Register(Component.For(item.Key, item.Value)
                    .ImplementedBy(item.Value)
                    .Named(item.Key.FullName)
                    .LifestyleSingleton());
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IList&lt;Type&gt;.</returns>
        private static IList<Type> GetInterfaces(Type @class)
        {
            return @class.GetInterfaces().Where(@interface => @interface.GetCustomAttribute(typeof(InjectInterfaceServiceAttribute), false) != null).ToList();
        }

        /// <summary>
        /// Gets the registration list.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>Dictionary&lt;Type, Type&gt;.</returns>
        private Dictionary<Type, Type> GetRegistrationList(IEnumerable<Assembly> assemblies)
        {
            var directList = new List<KeyValuePair<Type, Type>>();
            var inheritedList = new List<KeyValuePair<Type, Type>>();
            var classes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                classes.AddRange(
                    assembly.GetTypes()
                        .Where(
                            @object => @object.IsClass &&
                            !@object.IsAbstract &&
                            (
                                string.IsNullOrEmpty(_assemblyNamePrefix) ||
                                @object.Assembly.FullName.StartsWith(_assemblyNamePrefix,
                                StringComparison.CurrentCultureIgnoreCase)
                            )
                        ).ToList()
                );
            }

            var registrationList = new List<KeyValuePair<Type, Type>>();

            foreach (var @class in classes)
            {
                if (!string.IsNullOrEmpty(_assemblyNamePrefix) && !@class.Assembly.FullName.StartsWith(_assemblyNamePrefix, 
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var interfaces = GetInterfaces(@class);

                if(interfaces.Count == 0 || interfaces.Count > 2)
                {
                    continue;
                }

                Type @interface = null;

                if(interfaces.Count == 2)
                {
                    var @interface1 = interfaces[0];
                    var @interface2 = interfaces[1];

                    if(@interface1.GetInterfaces().FirstOrDefault(c=>c.FullName.Equals(@interface2.FullName)) != null) {
                        @interface = @interface1;
                    } else if (@interface2.GetInterfaces().FirstOrDefault(c => c.FullName.Equals(@interface1.FullName)) != null)
                    {
                        @interface = @interface2;
                    }
                }
                else
                {
                    @interface = interfaces[0];
                }

                if(@interface == null)
                {
                    continue;
                }

                registrationList.Add(new KeyValuePair<Type, Type>(@interface, @class));
            }

            registrationList = registrationList.OrderBy(c => c.Key.FullName).ToList();

            var result = new Dictionary<Type, Type>();

            foreach(var item in registrationList)
            {
                if(!result.ContainsKey(item.Key))
                {
                    result.Add(item.Key, item.Value);

                    continue;
                }

                var @existingClass = result[item.Key];
                var @currentClass = item.Value;
                var isExistingClassCustom = IsCustomService(@existingClass);
                var isCurrentClassCustom = IsCustomService(@currentClass);

                if(!isExistingClassCustom && isCurrentClassCustom)
                {
                    result[item.Key] = @currentClass;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether [is custom service] [the specified class].
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns><c>true</c> if [is custom service] [the specified class]; otherwise, <c>false</c>.</returns>
        private static bool IsCustomService(Type @class)
        {
            return @class.GetCustomAttributes(typeof(InjectServiceCustomAttribute), false).FirstOrDefault() != null;
        }


        /// <summary>
        /// Gets the assembly list.
        /// </summary>
        /// <param name="assemblyDescriptor">The assembly descriptor.</param>
        /// <returns>IList&lt;Assembly&gt;.</returns>
        private static IEnumerable<Assembly> GetAssemblyList(FromAssemblyDescriptor assemblyDescriptor)
        {
            var type = assemblyDescriptor.GetType();
            var assemblyField = type.GetField("assemblies", BindingFlags.NonPublic | BindingFlags.Instance);

            if (assemblyField == null)
            {
                return null;
            }

            var enumeration = (IEnumerable<Assembly>)assemblyField.GetValue(assemblyDescriptor);

            return enumeration?.ToList();
        }

        #endregion
    }
}
