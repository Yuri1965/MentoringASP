using MVCPeopleAwards.Helpers;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Controllers
{
    [Authorize(Roles = "CandidateAdmin")]
    public class EventUserController : Controller
    {
        private const string DEFAULT_BACK_ERROR_URL = "/EventsUser/Index";

        private IRepositoryAward repositoryAward;
        private IRepositoryPeople repositoryPeople;

        public EventUserController()
        {
            this.repositoryAward = new AwardsRepository();
            this.repositoryPeople = new PeopleRepository();
        }

        public EventUserController(IRepositoryAward repAward, IRepositoryPeople repPeople)
        {
            this.repositoryAward = repAward;
            this.repositoryPeople = repPeople;
        }

        public ActionResult Index()
        {
            try
            {
                EventsUserViewModel eventsModel = Session["EventsUserCandidate"] as EventsUserViewModel;
                if (eventsModel != null)
                {
                    ViewBag.Title = "Список изменений";
                    return View("Index", eventsModel);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return View("Error", ErrorHelper.GetErrorModel(e.Message, e.InnerException + e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }
            return RedirectToAction("Index", "PeoplesAward");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyOperation(string Key)
        {
            try
            {
                EventsUserViewModel eventsModel = Session["EventsUserCandidate"] as EventsUserViewModel;
                if (eventsModel != null)
                {
                    var eventExec = eventsModel.ListEvents.FirstOrDefault(x => x.KeyObjectStr == Key);
                    if (eventExec != null)
                    {
                        if (eventExec.TypeObject == EventObjectType.Award)
                        {
                            var model = eventExec.ObjectModel as AwardViewModel;
                            if (model != null)
                            {
                                if (eventExec.TypeOperation == EventOperationType.AddRecord)
                                {
                                    int saveID = repositoryAward.SaveAward(model);
                                    Logger.logger.Trace(String.Format("Добавлена награда:\n NameAward = {0}, DescriptionAward = {1}",
                                            model.NameAward, model.DescriptionAward));
                                }
                                if (eventExec.TypeOperation == EventOperationType.UpdateRecord)
                                {
                                    int saveID = repositoryAward.SaveAward(model);
                                    Logger.logger.Trace(String.Format("Изменена награда:\n Id={0}, NameAward = {1}, DescriptionAward = {2}",
                                            model.Id, model.NameAward, model.DescriptionAward));
                                }
                                if (eventExec.TypeOperation == EventOperationType.DeleteRecord)
                                {
                                    repositoryAward.DeleteAward(model.Id);
                                    Logger.logger.Trace(String.Format("Удалена награда:\n Id={0}", model.Id));
                                }
                            }
                        }
                    
                        if (eventExec.TypeObject == EventObjectType.People)
                        {
                            var model = eventExec.ObjectModel as PeopleViewModel;
                            if (model != null)
                            {
                                if (eventExec.TypeOperation == EventOperationType.AddRecord)
                                {
                                    int saveID = repositoryPeople.SavePeople(model);
                                    Logger.logger.Trace(String.Format("Добавлен человек:\n FirstName = {0}, LastName = {1}",
                                            model.FirstName, model.LastName));
                                }
                                if (eventExec.TypeOperation == EventOperationType.UpdateRecord)
                                {
                                    int saveID = repositoryPeople.SavePeople(model);
                                    Logger.logger.Trace(String.Format("Изменен человек:\n Id={0}, FirstName = {1}, LastName = {2}",
                                            model.Id, model.FirstName, model.LastName));
                                }
                                if (eventExec.TypeOperation == EventOperationType.DeleteRecord)
                                {
                                    repositoryPeople.DeletePeople(model.Id);
                                    Logger.logger.Trace(String.Format("Удален человек:\n Id={0}", model.Id));
                                }
                            }
                        }

                        eventsModel.DeleteEventFromList(eventExec);
                    }

                    if (eventsModel.ListEvents.Count == 0)
                    {
                        Session.Remove("EventsUserCandidate");
                        return RedirectToAction("Index", "PeoplesAward");
                    }
                    else
                    {
                        Session["EventsUserCandidate"] = eventsModel;
                        ViewBag.Title = "Список изменений";

                        if (Request.IsAjaxRequest())
                            return Json(new { Key });
                        else
                            return RedirectToAction("Index", "EventUser");
                    }
                }
            }
            catch (Exception e)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { error = e.InnerException.Message });
                else
                    return View("Error", ErrorHelper.GetErrorModel(e.Message, e.InnerException + e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }
            return RedirectToAction("Index", "PeoplesAward");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveOperation(string Key)
        {
            try
            {
                EventsUserViewModel eventsModel = Session["EventsUserCandidate"] as EventsUserViewModel;
                if (eventsModel != null)
                {
                    var eventExec = eventsModel.ListEvents.FirstOrDefault(x => x.KeyObjectStr == Key);
                    if (eventExec != null)
                        eventsModel.DeleteEventFromList(eventExec);

                    if (eventsModel.ListEvents.Count == 0)
                    {
                        Session.Remove("EventsUserCandidate");
                        return RedirectToAction("Index", "PeoplesAward");
                    }
                    else
                    {
                        Session["EventsUserCandidate"] = eventsModel;
                        ViewBag.Title = "Список изменений";

                        if (Request.IsAjaxRequest())
                            return Json(new { Key });
                        else
                            return RedirectToAction("Index", "EventUser");
                    }
                }
            }
            catch (Exception e)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { error = e.InnerException.Message });
                else
                    return View("Error", ErrorHelper.GetErrorModel(e.Message, e.InnerException + e.StackTrace, DEFAULT_BACK_ERROR_URL));
            }
            return RedirectToAction("PeoplesAward", "Index");
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