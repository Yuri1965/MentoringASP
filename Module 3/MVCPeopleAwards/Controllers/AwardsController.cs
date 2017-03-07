using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using MVCPeopleAwards.Models;
using MVCPeopleAwards.Repositories;
using System;

namespace MVCPeopleAwards.Controllers
{
    public class AwardsController : Controller
    {
        private AwardsRepository repository = new AwardsRepository();

        public ActionResult Index()
        {
            AwardsModelView awardsModel = new AwardsModelView();

            try
            {
                awardsModel.ListAwards = repository.GetListAwards();
            }
            catch
            {
                // создаем пустой список в случае неудачи и заполняем текст ошибки
                awardsModel.ListAwards = new List<AwardModel>();
                awardsModel.Error = "Не удалось получить список наград из БД";
            }

            return View(awardsModel);
        }

        public ActionResult GetEdit(int? id)
        {
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AwardModel awardModel;
            try
            {
                awardModel = repository.GetAward(id);
                if (awardModel == null)
                    return HttpNotFound();
            }
            catch
            {
                return HttpNotFound();
            }

            return View(awardModel);
        }

        public ActionResult GetCreate()
        {
            AwardModel awardModel;
            try
            {
                awardModel = repository.GetNewAward();
                if (awardModel == null)
                    return HttpNotFound();
            }
            catch
            {
                return HttpNotFound();
            }

            return View(awardModel);
        }

        public ActionResult GetDelete(int? id)
        {
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AwardModel awardModel;
            try
            {
                awardModel = repository.GetAward(id);
                if (awardModel == null)
                    return HttpNotFound();
            }
            catch
            {
                return HttpNotFound();
            }

            return View(awardModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCreate([Bind(Include = "NameAward,DescriptionAward")] AwardModel awardModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveAward(awardModel, true);
                    return RedirectToAction("Index");
                }
                catch
                {
                }
            }
            return RedirectToAction("GetCreate", awardModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveEdit([Bind(Include = "Id,NameAward,DescriptionAward")] AwardModel awardModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveAward(awardModel, false);
                    return RedirectToAction("Index");
                }
                catch
                {
                }
            }
            return RedirectToAction("GetEdit", awardModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                repository.DeleteAward(id);
            }
            catch
            {
                AwardModel awardModel;
                awardModel = repository.GetAward(id);
                if (awardModel == null)
                    return HttpNotFound();
                else
                {
                    awardModel.Error = "Запись не удалена! Возможно на нее есть ссылки в списке награжденных";
                    return View("GetDelete", awardModel);
                }
            }
            return RedirectToAction("Index");
        }

        // проверка на уникальность наименования
        [HttpPost]
        public ActionResult CheckNameAward(string nameAward, int id = 0)
        {
            try
            {
                if (repository.CheckNameAward(nameAward, id))
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch
            {
            }
            return Json(true, JsonRequestBehavior.AllowGet);
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
