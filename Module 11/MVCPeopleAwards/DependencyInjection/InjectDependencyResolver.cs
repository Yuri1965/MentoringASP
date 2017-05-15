using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCPeopleAwards.Repositories;
using MVCPeopleAwards.Models;

namespace MVCPeopleAwards.DependencyInjection
{
    public class InjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public InjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        // здесь биндим все наши типы, которые нужны
        private void AddBindings()
        {
            kernel.Bind<IRepositoryPeople>().To<PeopleRepository>();
            kernel.Bind<IRepositoryAward>().To<AwardsRepository>();
        }
    }
}