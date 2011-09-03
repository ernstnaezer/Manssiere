namespace Manssiere
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Core;
    using Core.DemoFlow;
    using Core.Rendering;
    using Microsoft.Practices.ServiceLocation;

    internal class Program : Application
    {
        private readonly IWindsorContainer _container = new ApplicationContainer();
        private OpenglRendering _openglRendering;

        /// <summary>
        ///   Initializes a new instance of the
        ///   <see cref = "Program" />
        ///   class.
        /// </summary>
        /// <exception cref = "T:System.InvalidOperationException">More than one instance of the
        ///   <see cref = "T:System.Windows.Application" />
        ///   class is created per
        ///   <see cref = "T:System.AppDomain" />
        ///   .</exception>
        private Program()
        {
            // this is an empty event handler to trigger animation updates inside the WPF framework.
            CompositionTarget.Rendering += (cs, ce) => { };

            Startup += ApplicationStartup;
            Exit += ApplicationExit;
        }

        /// <summary>
        ///   Program entry point
        /// </summary>
        /// <param name = "args"></param>
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new Program();
            app.DispatcherUnhandledException += AppDispatcherUnhandledException;
            app.Run();
        }

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Console.Out.WriteLine("e = {0}", e.Exception.Message);
        }

        /// <summary>
        ///   Applications startup event.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The
        ///   <see cref = "System.Windows.StartupEventArgs" />
        ///   instance containing the event data.</param>
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            var serviceLocator = new WindsorAdapter(_container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            _container.Register(Component
                                    .For<AbstractDemoFlow>()
                                    .ImplementedBy<DemoFlow>());

            Configuration.WindowTitle = "coulrophobia is not to be laughed at";

            Dispatcher
                .CurrentDispatcher
                .BeginInvoke(DispatcherPriority.Background,
                             (Action) (() =>
                                           {
                                               _openglRendering = new OpenglRendering();
                                               _openglRendering.Run(60);
                                               Shutdown();

                                               _container.Dispose();
                                           }));

            // Start the worker thread's message pump so that
            // queued messages are processed.
            Dispatcher.Run();
        }

        /// <summary>
        ///   Applications exit event.
        /// </summary>
        /// <param name = "sender">The sender.</param>
        /// <param name = "e">The
        ///   <see cref = "System.Windows.ExitEventArgs" />
        ///   instance containing the event data.</param>
        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            _openglRendering.Exit();
        }
    }
}