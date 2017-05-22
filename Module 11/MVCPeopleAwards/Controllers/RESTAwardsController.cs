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
        private IRepositoryAward repositoryAward;
        private IRepositoryPeople repositoryPeople;

        public RESTAwardsController()
        {
            this.repositoryAward = new AwardsRepository();
            this.repositoryPeople = new PeopleRepository();
        }

        public RESTAwardsController(IRepositoryAward repAward, IRepositoryPeople repPeople)
        {
            this.repositoryAward = repAward;
            this.repositoryPeople = repPeople;
        }

        [HttpGet]
        [Route("api/awards")]
        [EnableQuery]
        public IQueryable<AwardViewModel> Get()
        {
            ListAwardsViewModel awardsModel = new ListAwardsViewModel();
            try
            {
                awardsModel.ListAwards = (List<AwardViewModel>)repositoryAward.GetListAward();
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
                awardsModel.ListAwards = (List<AwardViewModel>)repositoryAward.GetListAward(nameAward);
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
                awardModel = repositoryAward.GetAwardById(id);

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
                awardModel = repositoryAward.GetAwardByName(nameAward);

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
        public IHttpActionResult CheckNameAward(string nameAward, int id)
        {
            if (id < 0) id = 0;

            if (repositoryAward.CheckNameAward(nameAward, id))
                return Json(false);

            return Json(true);
        }

        [HttpDelete]
        [Route("api/award/delete/{id:int:min(1)}")]
        public IHttpActionResult DeleteAward(int id)
        {
            AwardViewModel awardModel;
            try
            {
                awardModel = repositoryAward.GetAwardById(id);

                if (awardModel == null)
                    return NotFound();

                repositoryAward.DeleteAward(id);
                Logger.logger.Trace(String.Format("Удалена награда:\n Id={0}", id));
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return NotFound();
            }
            return Ok(awardModel.Id);
        }

        [HttpPost]
        [Route("api/award/create")]
        public IHttpActionResult CreateAward([FromBody]AwardViewModel awardModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int saveID = repositoryAward.SaveAward(awardModel);
                    Logger.logger.Trace(String.Format("Добавлена награда:\n NameAward = {0}, DescriptionAward = {1}",
                            awardModel.NameAward, awardModel.DescriptionAward));

                    return Ok(saveID);
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                    return BadRequest(e.Message);
                }
            }
            else return BadRequest(ModelState);
        }

        public IHttpActionResult CreatePeopleAward(int peopleId, int awardId)
        {
            if (peopleId <= 0 || awardId <= 0)
            {
                return BadRequest("Переданы неверные параметры"); 
            }

            if (ModelState.IsValid)
            {
                PeopleViewModel peopleModel;
                peopleModel = GetPeopleModelForEdit(peopleId);
                if (peopleModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                try
                {
                    repositoryAward.SavePeopleAward(peopleId, awardId);
                    Logger.logger.Trace(String.Format("Добавлена награда человека:\n PeopleID={0}, AwardID={1}", peopleId, awardId));

                    peopleModel = GetPeopleModelForEdit(peopleId);
                    if (peopleModel == null)
                        return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    peopleModel.Error = "Запись не добавлена! Ошибка на сервере";
                }

                return RedirectToAction("EditPeopleAwards", new { id = peopleId });
            }
            else
            {
                PeopleViewModel peopleModel = GetPeopleModelForEdit(peopleId);
                if (peopleModel == null)
                    return View("Error", ErrorHelper.GetErrorModel("Не найден человек с таким идентификатором", "", DEFAULT_BACK_ERROR_URL));

                return View("EditPeopleAwards", peopleModel);
            }
        }

        [HttpPut]
        [Route("api/award/update")]
        public IHttpActionResult UpdateAward([FromBody]AwardViewModel awardModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int saveID = repositoryAward.SaveAward(awardModel);
                    Logger.logger.Trace(String.Format("Изменена награда:\n Id={0}, NameAward = {1}, DescriptionAward = {2}",
                            awardModel.Id, awardModel.NameAward, awardModel.DescriptionAward));

                    return Ok(saveID);
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                    return BadRequest(e.Message);
                }
            }
            else return BadRequest(ModelState);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)repositoryAward).Dispose();
                ((IDisposable)repositoryPeople).Dispose();
            }
            base.Dispose(disposing);
        }
    }
}