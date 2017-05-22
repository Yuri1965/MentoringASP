using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Routing.Constraints;
using System.Web.Http.OData;
using System.Net.Http;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using RazorEngine.Configuration;
using MVCPeopleAwards.Helpers;
using System.Net.Http.Headers;

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
        [Route("api/awards")]
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

        [HttpGet]
        [Route("api/awards/{nameAward}")]
        [EnableQuery]
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

        [HttpGet]
        [Route("api/award/{id:int:min(1)}", Order = 1)]
        public IHttpActionResult GetAwardById(int id)
        {
            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAwardById(id);

                if (awardModel == null)
                    return NotFound();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return NotFound();
            }
            return Ok(awardModel);
        }

        [HttpGet]
        [Route("api/award/{nameAward:regex(^([a-zA-Zа-яА-Я0-9]+)$)}", Order = 2)]
        public IHttpActionResult GetAwardByName(string nameAward)
        {
            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAwardByName(nameAward);

                if (awardModel == null)
                    return NotFound();
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return NotFound();
            }
            return Ok(awardModel);
        }

        // проверка на уникальность наименования
        [HttpGet]
        [Route("api/award/{id}/checkName/{nameAward:regex(^([a-zA-Zа-яА-Я0-9]+)$)}")]
        public bool CheckNameAward(string nameAward, int id)
        {
            if (id < 0) id = 0;

            if (repository.CheckNameAward(nameAward, id))
                return false;

            return true;
        }

        [HttpDelete]
        [Route("api/award/delete/{id:int:min(1)}")]
        public IHttpActionResult DeleteAward(int id)
        {
            AwardViewModel awardModel;
            try
            {
                awardModel = repository.GetAwardById(id);

                if (awardModel == null)
                    return NotFound();

                repository.DeleteAward(id);
                Logger.logger.Trace(String.Format("Удалена награда:\n Id={0}", id));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return NotFound();
            }
            return Ok(awardModel.Id);
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