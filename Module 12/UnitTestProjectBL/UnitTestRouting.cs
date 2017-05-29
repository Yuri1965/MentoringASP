using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.Web.Routing;
using MVCPeopleAwards;

namespace UnitTestProjectBL
{
    [TestClass]
    public class UnitTestRouting
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            RouteConfig.RegisterAllRoutes(RouteTable.Routes);
        }

        [TestMethod]
        public void TestRouteBaseRoot()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(item => item.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~/");

            var routeData = RouteTable.Routes.GetRouteData(context.Object);

            Assert.AreEqual("Index", routeData.Values["action"]);
            Assert.AreEqual("PeoplesAward", routeData.Values["controller"]);
        }

        [TestMethod]
        public void TestRoutePeoplesUrl()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(item => item.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~/peoples");

            var routeData = RouteTable.Routes.GetRouteData(context.Object);

            Assert.AreEqual("Index".ToUpper(), routeData.Values["action"].ToString().ToUpper());
            Assert.AreEqual("PeoplesAward", routeData.Values["controller"]);
        }

        [TestMethod]
        public void TestRoutePeoplesByNameUrl()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(item => item.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~/peoplesByName/Name");

            var routeData = RouteTable.Routes.GetRouteData(context.Object);

            Assert.AreEqual("GetPeoplesByName".ToUpper(), routeData.Values["action"].ToString().ToUpper());
            Assert.AreEqual("PeoplesAward", routeData.Values["controller"]);
        }

        [TestMethod]
        public void TestRoutePeopleCreateUrl()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(item => item.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~/create-people");

            var routeData = RouteTable.Routes.GetRouteData(context.Object);

            Assert.AreEqual("CreatePeople".ToUpper(), routeData.Values["action"].ToString().ToUpper());
            Assert.AreEqual("PeoplesAward", routeData.Values["controller"]);
        }

        [TestMethod]
        public void TestRoutePeopleEditUrl()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(item => item.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~/people/3/edit");

            var routeData = RouteTable.Routes.GetRouteData(context.Object);

            Assert.AreEqual("EditPeople".ToUpper(), routeData.Values["action"].ToString().ToUpper());
            Assert.AreEqual("PeoplesAward", routeData.Values["controller"]);
        }

        [TestMethod]
        public void TestRoutePeopleWithAwardsUrl()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(item => item.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~/people/3");

            var routeData = RouteTable.Routes.GetRouteData(context.Object);

            Assert.AreEqual("EditPeopleAwards".ToUpper(), routeData.Values["action"].ToString().ToUpper());
            Assert.AreEqual("PeoplesAward", routeData.Values["controller"]);
        }

        [TestMethod]
        public void TestRoutePeopleByFullNameUrl()
        {
            var context = new Mock<HttpContextBase>();
            context.Setup(item => item.Request.AppRelativeCurrentExecutionFilePath).Returns(@"~/people/FirstName_LastName");

            var routeData = RouteTable.Routes.GetRouteData(context.Object);

            Assert.AreEqual("GetPeopleByFullName".ToUpper(), routeData.Values["action"].ToString().ToUpper());
            Assert.AreEqual("PeoplesAward", routeData.Values["controller"]);
        }
    }
}
