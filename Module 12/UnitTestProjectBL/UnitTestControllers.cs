using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCPeopleAwards.Controllers;
using System.Web.Mvc;
using System.Web;
using Moq;
using System.Net;
using System.Web.Routing;
using MVCPeopleAwards.Models;

namespace UnitTestProjectBL
{
    [TestClass]
    public class UnitTestControllers
    {
        [TestMethod]
        public void TestGetIndexAwаrds()
        {
            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.Request).Returns(request.Object);

            var awardsController = new AwardsController();
            awardsController.ControllerContext = new ControllerContext(context.Object, new RouteData(), awardsController);
            var result = awardsController.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Index", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница со списком наград сформирована");
        }

        [TestMethod]
        public void TestGetIndexAwаrdsAjax()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(req => req.Headers).Returns(new WebHeaderCollection()
            {
                { "X-Requested-With", "XMLHttpRequest"}
            });
            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.Request).Returns(request.Object);

            var awardsController = new AwardsController();
            awardsController.ControllerContext = new ControllerContext(context.Object, new RouteData(), awardsController);
            var result = awardsController.Index();
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual("ListAwardsPartial", ((PartialViewResult)result).ViewName);
            Console.WriteLine("Частичное представление для страницы со списком наград сформировано");
        }

        [TestMethod]
        public void TestGetIndexAwаrdsByName()
        {
            string nameAward = "wwww";

            var awardsController = new AwardsController();
            var result = awardsController.GetAwardsByName(nameAward);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Index", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница со списком наград с Наименованием = {0} сформирована", nameAward);
        }

        [TestMethod]
        public void TestGetAwаrdById()
        {
            var awardsController = new AwardsController();

            //проверим которая существует
            int idAward = 2;
            var result = awardsController.GetAwardById(idAward);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("AwardDetail", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница награды с Id = {0} сформирована", idAward);

            //проверим которая НЕ существует
            idAward = 10000;
            result = awardsController.GetAwardById(idAward);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Error", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница награды с Id = {0} НЕ существует", idAward);
        }

        [TestMethod]
        public void TestGetAwаrdInfo()
        {
            var awardsController = new AwardsController();

            //проверим которая существует
            int idAward = 2;
            var result = awardsController.GetAwardInfo(idAward);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual("ModalAwardDetail", ((PartialViewResult)result).ViewName);
            Console.WriteLine("Данные для модального окна с информацией о награде с Id = {0} сформированы", idAward);

            //проверим которая НЕ существует
            idAward = 10000;
            result = awardsController.GetAwardInfo(idAward);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Error", ((ViewResult)result).ViewName);
            Console.WriteLine("Данные для модального окна с информацией о награде с Id = {0} НЕ существуют", idAward);
        }

        [TestMethod]
        public void TestGetAwаrdByName()
        {
            var awardsController = new AwardsController();

            //проверим которая существует
            string nameAward = "Nagrada 2";
            var result = awardsController.GetAwardByName(nameAward);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("AwardDetail", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница награды с Наименованием = {0} сформирована", nameAward);

            //проверим которая НЕ существует
            nameAward = "ghfadgahga";
            result = awardsController.GetAwardByName(nameAward);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Error", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница награды с Наименованием = {0} НЕ существует", nameAward);
        }

        [TestMethod]
        public void TestGetEditAward()
        {
            var awardsController = new AwardsController();

            //проверим которая существует
            int idAward = 2;
            var result = awardsController.EditAward(idAward);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("CreateEditAward", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница награды с Id = {0} сформирована", idAward);

            //проверим которая НЕ существует
            idAward = 10000;
            result = awardsController.EditAward(idAward);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Error", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница награды с Id = {0} НЕ существует", idAward);
        }

        [TestMethod]
        //комплексный тест: Добавляет награду, меняет ее имя, потом удаляет ее
        public void TestComplexAward()
        {
            string nameAward = "Test award 1";
            //добавим Награду
            AwardViewModel award = new AwardViewModel()
            {
                Id = 0,
                ImageIsEmpty = true,
                PhotoAward = null,
                PhotoMIMEType = "",
                NameAward = nameAward,
                DescriptionAward = "Description Test award 1",
            };

            var request = new Mock<HttpRequestBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(ctx => ctx.Request).Returns(request.Object);

            var awardsController = new AwardsController();
            awardsController.ControllerContext = new ControllerContext(context.Object, new RouteData(), awardsController);
            var result = awardsController.SaveAward(award);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
            Assert.AreEqual("AwardSinglePartial", ((PartialViewResult)result).ViewName);
            //получим индентификатор добавленной награды
            award = (AwardViewModel)(((PartialViewResult)result).Model);
            int saveId = award.Id;
            Assert.IsTrue(saveId > 0);
            Assert.AreEqual(award.NameAward, nameAward);
            Console.WriteLine("Частичное представление для новой награды сформировано, Id = {0}, Name = {1}", saveId, award.NameAward);

            //изменим название награды 
            nameAward = "Test award 232421";
            award.NameAward = nameAward;
            result = awardsController.SaveAward(award);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["Action"]);
            Console.WriteLine("Редирект на метод Index со списком наград");
            //получим измененную награду
            result = awardsController.GetAwardById(saveId);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("AwardDetail", ((ViewResult)result).ViewName);
            Console.WriteLine("Страница награды с Id = {0} сформирована", saveId);
            award = (AwardViewModel)(((ViewResult)result).Model);
            Assert.AreEqual(award.NameAward, nameAward);
            Console.WriteLine("Наименование Награды изменено: Id = {0}, Наименование = {1}", saveId, award.NameAward);

            //удалим добавленную награду
            result = awardsController.DeleteAward(saveId);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["Action"]);
            Console.WriteLine("Редирект на метод Index со списком наград");
            //проверим наличие награды
            result = awardsController.GetAwardById(saveId);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Error", ((ViewResult)result).ViewName);
            Console.WriteLine("Запись награды с Id = {0} удалена", saveId);

        }
    }
}
