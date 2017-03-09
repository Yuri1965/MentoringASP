using MVCPeopleAwards.Enums;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Controllers
{
    public class PeoplesAwardController : Controller
    {
        private IRepositoryPeople repository;

        public PeoplesAwardController(IRepositoryPeople rep)
        {
            this.repository = rep;
        }

        public ActionResult Index()
        {
            PeopleModelView peopleModel = new PeopleModelView();

            try
            {
                peopleModel.ListPeople = repository.GetListPeople().ToList();
            }
            catch
            {
                // создаем пустой список в случае неудачи и заполняем текст ошибки
                peopleModel.ListPeople = new List<PeopleModel>();
                peopleModel.Error = "Не удалось получить список награжденных из БД";
            }

            return View(peopleModel);
        }

        public ActionResult CreatePeople()
        {
            PeopleModel peopleModel = new PeopleModel()
            {
                Id = 0,
                FirstName = "",
                LastName = "",
                BirthDate = DateTime.Now.Date.AddYears(-16),
                PeopleAwards = new List<PeopleAwardsModel>()
            };
            return View(peopleModel);
        }

        public ActionResult EditPeople(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                if (peopleModel == null)
                    return HttpNotFound("Не найден человек с таким идентификатором");
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }

            return View(peopleModel);
        }

        public ActionResult DeletePeople(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                if (peopleModel == null)
                    return HttpNotFound("Не найден человек с таким идентификатором");
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }

            return View(peopleModel);
        }

        public ActionResult EditPeopleAwards(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleModel peopleModel;
            try
            {
                peopleModel = repository.GetPeople(id);
                if (peopleModel == null)
                    return HttpNotFound("Не найден человек с таким идентификатором");
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }

            return View(peopleModel);
        }

        public ActionResult CreatePeopleAward(int peopleId)
        {
            if (peopleId <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            PeopleModel peopleModel;
            List<AwardModel> awardsModel;
            try
            {
                peopleModel = repository.GetPeople(peopleId);
                if (peopleModel == null)
                    return HttpNotFound("Не найден человек с таким идентификатором");
                awardsModel = repository.GetAwards();
                if (awardsModel == null || awardsModel.Count() == 0)
                    return HttpNotFound("Справочник Награды не заполнен или не удалось загрузить данные из БД");
                ViewBag.PeopleModel = peopleModel;
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }

            return View(awardsModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCreatePeople([Bind(Include = "LastName,FirstName,BirthDate")] PeopleModel peopleModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // проверка на ввод Даты рождения (возраст от 5 до 120 лет)
                    if (!UtilHelper.CheckBirthDate(peopleModel.BirthDate))
                    {
                        peopleModel.Error = "Возраст может быть от 5 до 120 лет! Введите корректную дату рождения";
                        return View("CreatePeople", peopleModel);
                    }

                    repository.SavePeople(peopleModel, Operation.Add);
                    Logger.logger.Info(String.Format("Добавлен человек:\n Id={0}, LastName={1}, FirstName={2}, BirthDate={3}",
                            peopleModel.Id, peopleModel.LastName, peopleModel.FirstName, peopleModel.BirthDateStr));
                    return RedirectToAction("Index");
                }
                catch
                {
                }
            }
            return RedirectToAction("CreatePeople", peopleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEditPeople([Bind(Include = "Id,LastName,FirstName,BirthDate")] PeopleModel peopleModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // проверка на ввод Даты рождения (возраст от 5 до 120 лет)
                    if (!UtilHelper.CheckBirthDate(peopleModel.BirthDate))
                    {
                        peopleModel.Error = "Возраст может быть от 5 до 120 лет! Введите корректную дату рождения";
                        return View("EditPeople", peopleModel);
                    }

                    repository.SavePeople(peopleModel, Operation.Update);
                    Logger.logger.Info(String.Format("Изменен человек:\n Id={0}, LastName={1}, FirstName={2}, BirthDate={3}",
                            peopleModel.Id, peopleModel.LastName, peopleModel.FirstName, peopleModel.BirthDateStr));
                    return RedirectToAction("Index");
                }
                catch
                {
                }
            }
            return RedirectToAction("EditPeople", peopleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDeletePeople(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                repository.DeletePeople(id);
                Logger.logger.Info(String.Format("Удален человек:\n Id={0}", id));
            }
            catch
            {
                return HttpNotFound("Ошибка на сервере");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePeopleAward(int Id, int peopleID)
        {
            if (Id <= 0 || peopleID <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Переданы некорректные параметры");
            }

            try
            {
                repository.DeletePeopleAward(Id);
                Logger.logger.Info(String.Format("Удалена награда человека:\n Id={0}, PeopleID={1}", Id, peopleID));
            }
            catch
            {
                PeopleModel peopleModel;
                try
                {
                    peopleModel = repository.GetPeople(peopleID);
                    if (peopleModel == null)
                        return HttpNotFound("Не найден человек с таким идентификатором");
                    else
                    {
                        peopleModel.Error = "Запись не удалена! Ошибка на сервере";
                        return View("EditPeopleAwards", peopleModel);
                    }
                }
                catch
                {
                }
                return HttpNotFound("Ошибка на сервере");
            }
            return RedirectToAction("EditPeopleAwards");
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