using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;

namespace MVCPeopleAwards.Controllers
{
    //[Authorize(Roles = "Admin,CandidateAdmin")]
    public class RESTAwardsController : ApiController
    {
        private IRepositoryAward repository;

        public RESTAwardsController()
        {
            this.repository = new AwardsRepository();
        }

        public RESTAwardsController(IRepositoryAward rep)
        {
            this.repository = rep;
        }

        [HttpGet]
        [EnableQuery]
        public IQueryable<AwardViewModel> Get()
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                awardsModel.ListAwards = (List<AwardViewModel>)repository.GetListAward();
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                // создаем пустой список в случае неудачи
                awardsModel.ListAwards = new List<AwardViewModel>();
            }
            return new EnumerableQuery<AwardViewModel>(awardsModel.ListAwards);
        }

        [Route("api/awards/{nameAward:regex(^([a-zA-Zа-яА-Я0-9 -]+)$)}")]
        [EnableQuery]
        [HttpGet]
        public IQueryable<AwardViewModel> GetAwardsByName(string nameAward)
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                awardsModel.ListAwards = (List<AwardViewModel>)repository.GetListAward(nameAward);
            }
            catch (Exception e)
            {
                Logger.LogException(e);

                // создаем пустой список в случае неудачи
                awardsModel.ListAwards = new List<AwardViewModel>();
            }
            return new EnumerableQuery<AwardViewModel>(awardsModel.ListAwards);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)repository).Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
