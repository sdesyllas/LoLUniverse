using System;
using System.Configuration;
using System.Web;
using LoLUniverse;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using RiotApi.Net.RestClient;
using LoLUniverse.Services;
using Raven.Client.Document;
using Raven.Client;
using LoLUniverse.NinjectModules;
using System.Web.Http;
using Ninject.Activation;
using System.Collections.Generic;
using Ninject.Syntax;
using System.Linq;
using Ninject.Parameters;
using System.Web.Http.Dependencies;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace LoLUniverse
{
    public static class NinjectWebCommon 
    {
        public class NinjectResolver : NinjectScope, IDependencyResolver
        {
            private IKernel _kernel;
            public NinjectResolver(IKernel kernel) : base(kernel)
            {
                _kernel = kernel;
            }
            public IDependencyScope BeginScope()
            {
                return new NinjectScope(_kernel.BeginBlock());
            }
        }

        public class NinjectScope : IDependencyScope
        {
            protected IResolutionRoot resolutionRoot;
            public NinjectScope(IResolutionRoot kernel)
            {
                resolutionRoot = kernel;
            }
            public object GetService(Type serviceType)
            {
                IRequest request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
                return resolutionRoot.Resolve(request).SingleOrDefault();
            }
            public IEnumerable<object> GetServices(Type serviceType)
            {
                IRequest request = resolutionRoot.CreateRequest(serviceType, null, new Parameter[0], true, true);
                return resolutionRoot.Resolve(request).ToList();
            }
            public void Dispose()
            {
                IDisposable disposable = (IDisposable)resolutionRoot;
                if (disposable != null) disposable.Dispose();
                resolutionRoot = null;
            }
        }

        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            // bind riot client
            kernel.Bind<IRiotClient>().To<RiotClient>().WithConstructorArgument(ConfigurationManager.AppSettings["RiotApiKey"]);
            // bind NoSql store
            kernel.Load<RavenModule>();
            // bind cache manager
            kernel.Bind<ICacheManager>().To<RavenCacheManager>();
        }        
    }
}
