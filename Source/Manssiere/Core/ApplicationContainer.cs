namespace Manssiere.Core
{
    using System.Windows;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using DomainEvents;
    using Graphics.Transition;
    using Infrastructure;

    /// <summary>
    /// Demo DI container
    /// </summary>
    public class ApplicationContainer : WindsorContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationContainer"/> class.
        /// </summary>
        public ApplicationContainer()
        {
            InitializeContainer();
        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        private void InitializeContainer()
        {
            Register(AllTypes.FromAssembly(GetType().Assembly)
                         .Where(t => t.Name.EndsWith("Impl"))
                         .WithService.FirstInterface());

            Kernel.ComponentCreated += KernelComponentCreated;

            Register(AllTypes
                         .FromAssembly(GetType().Assembly)
                         .Where(type => typeof(FrameworkElement).IsAssignableFrom(type)));

            Register(AllTypes.Of(typeof (ITransition)).FromAssembly(GetType().Assembly));
            Register(AllTypes.Of(typeof (IHandles<>)).FromAssembly(GetType().Assembly));
        }

        private static void KernelComponentCreated(Castle.Core.ComponentModel model, object instance)
        {
            if(instance is FrameworkElement == false) return;
            XamlLoader.InitializeFrameworkElement((FrameworkElement) instance);
        }
    }
}