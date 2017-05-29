using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorGenerator.Testing;
using MVCPeopleAwards.Repositories;
using MVCPeopleAwards.Models;
using System.Collections.Generic;
using ASP;

namespace UnitTestProjectBL
{
    [TestClass]
    public class UnitTestView
    {
        private AwardsRepository awardRepo = new AwardsRepository();

        [TestMethod]
        public void TestIndexAward()
        {
            var lstAwards = (List<AwardViewModel>)awardRepo.GetListAward();
            var awardsViewModel = new ListAwardsViewModel();
            awardsViewModel.ListAwards = lstAwards;

            var viewIndex = new _Views_Awards_Index_cshtml();
            viewIndex.ViewBag.Title = "Список наград Test";
            var htmlIndex = viewIndex.RenderAsHtml(awardsViewModel);
            var node = htmlIndex.DocumentNode.Element("h2");
            Assert.AreEqual("Список наград Test", node.InnerText);
        }
    }
}
