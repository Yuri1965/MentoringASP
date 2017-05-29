using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVCPeopleAwards.Repositories;
using MVCPeopleAwards.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace UnitTestProjectBL
{
    [TestClass]
    public class UnitTestBL
    {
        private AwardsRepository awardRepo = new AwardsRepository();
        private PeopleRepository peopleRepo = new PeopleRepository();

        [TestMethod]
        public void TestGetListAward()
        {
            //получим весь список 
            var lstAwardsAll = (List<AwardViewModel>)awardRepo.GetListAward();
            Assert.IsTrue(lstAwardsAll.Count >= 0);
            Console.WriteLine("Всего записей: {0}", lstAwardsAll.Count);

            //получим список ограниченный по заданному Наименованию
            string nameAward = "wwwwww1";
            var lstAwardsPart = (List<AwardViewModel>)awardRepo.GetListAward(nameAward);
            Assert.IsTrue((lstAwardsAll.Count) >= (lstAwardsPart.Count));
            Console.WriteLine("Записей с Наименованием = \"{0}\": {1}", nameAward, lstAwardsPart.Count);
        }

        [TestMethod]
        public void TestGetListPeople()
        {
            //получим весь список 
            var lstPeopleAll = (List<PeopleViewModel>)peopleRepo.GetListPeople();
            Assert.IsTrue(lstPeopleAll.Count >= 0);
            Console.WriteLine("Всего записей: {0}", lstPeopleAll.Count);

            //получим список ограниченный по заданному Имени
            string namePeople = "Сидор";
            var lstPeoplePart = (List<PeopleViewModel>)peopleRepo.GetListPeople(namePeople);
            Assert.IsTrue((lstPeopleAll.Count) >= (lstPeoplePart.Count));
            Console.WriteLine("Записей с Именем = \"{0}\": {1}", namePeople, lstPeoplePart.Count);
        }

        [TestMethod]
        public void TestGetAwardById()
        {
            //получим награду по Id который есть
            int Id = 10;
            var awardsById = awardRepo.GetAwardById(Id);
            Assert.AreEqual(awardsById.Id, Id);
            Console.WriteLine("Найдена награда с Id = {0}", awardsById.Id);

            //поищем награду по Id которого НЕТ
            Id = 10000;
            awardsById = awardRepo.GetAwardById(Id);
            Assert.IsNull(awardsById);
            Console.WriteLine("Награда с Id = {0} НЕ найдена", Id);
        }

        [TestMethod]
        public void TestGetPeopleById()
        {
            //получим человека по Id который есть
            int Id = 7;
            var peopleById = peopleRepo.GetPeople(Id);
            Assert.AreEqual(peopleById.Id, Id);
            Console.WriteLine("Найден человек с Id = {0}", peopleById.Id);

            //поищем человека по Id которого НЕТ
            Id = 10000;
            peopleById = peopleRepo.GetPeople(Id);
            Assert.IsNull(peopleById);
            Console.WriteLine("Человек с Id = {0} НЕ найден", Id);
        }

        [TestMethod]
        public void TestGetPeopleByFullName()
        {
            //получим человека по Полному имени который есть
            string nameFull = "Сидор_Сидоров";
            var peopleByName = peopleRepo.GetPeopleByFullName(nameFull);
            Assert.AreEqual(peopleByName.FirstName + "_" + peopleByName.LastName, nameFull);
            Console.WriteLine("Найден человек с Именем = \"{0}\" и Фамилией = \"{1}\"", peopleByName.FirstName, peopleByName.LastName);

            //поищем человека по Полному имени которого НЕТ
            var firstName = "Гена";
            var lastName = "Зюганов";
            nameFull = firstName + "_" + lastName;
            peopleByName = peopleRepo.GetPeopleByFullName(nameFull);
            Assert.IsNull(peopleByName);
            Console.WriteLine("Человек с Именем = \"{0}\" и Фамилией = \"{1}\" НЕ найден", firstName, lastName);
        }

        [TestMethod]
        public void TestGetAwardByName()
        {
            //получим награду по Наименованию которое есть
            string nameAward = "Nagrada 2";
            var awardsByName = awardRepo.GetAwardByName(nameAward);
            Assert.AreEqual(awardsByName.NameAward, nameAward);
            Console.WriteLine("Найдена награда с Наименованием = \"{0}\"", awardsByName.NameAward);

            //поищем награду по Наименованию которого НЕТ
            nameAward = "пафврпафрпафврпа Nagrada 2";
            awardsByName = awardRepo.GetAwardByName(nameAward);
            Assert.IsNull(awardsByName);
            Console.WriteLine("Награда с Наименованием = \"{0}\" НЕ найдена", nameAward);
        }

        public void CheckPeopleAward()
        {
            //поищем награду награду у человека, которая есть
            int idAward = 3;
            int idPeople = 2;

            //должно вернуть true
            Assert.IsTrue(peopleRepo.CheckPeopleAward(idAward, idPeople));
            Console.WriteLine("Найдена награда c Id = {0} у человека с Id = \"{1}\"", idAward, idPeople);

            //поищем награду награду у человека, которой НЕТ
            idAward = 100000;
            //должно вернуть false
            Assert.IsTrue(peopleRepo.CheckPeopleAward(idAward, idPeople));
            Console.WriteLine("Награды с Id = \"{0}\" у человека с Id = {1} НЕТ", idAward, idPeople);
        }

        [TestMethod]
        public void TestCheckNameAward()
        {
            //поищем награду по Наименованию которое есть БЕЗ передачи Id
            string nameAward = "Nagrada 2";
            //должно вернуть true
            Assert.IsTrue(awardRepo.CheckNameAward(nameAward));
            Console.WriteLine("Найдена награда с Наименованием = \"{0}\"", nameAward);

            //поищем награду по Наименованию которое есть C передачей Id (запись 1 в БД такая), 
            int Id = 2;
            nameAward = "Nagrada 2";
            //должно вернуть false
            Assert.IsFalse(awardRepo.CheckNameAward(nameAward, Id));
            Console.WriteLine("Награда с Наименованием = \"{0}\" и Id = {1} уникальна в БД", nameAward, Id);
        }

        [TestMethod]
        public void TestGetListAwardRef()
        {
            //получим весь список справочника Награды
            var lstAwardsAll = peopleRepo.GetAwards().ToList();
            Assert.IsTrue(lstAwardsAll.Count >= 0);
            Console.WriteLine("Всего записей: {0}", lstAwardsAll.Count);
        }

        [TestMethod]
        //комплексный тест - добавляет Награду человеку и потом Удаляет эту награду у человека
        public void TestComplexPeopleAward()
        {
            int saveId = 0;

            //добавим награду человеку
            int idAward = 2;
            int idPeople = 1;
            saveId = peopleRepo.SavePeopleAward(idPeople, idAward);
            //должно вернуть true
            Assert.IsTrue(peopleRepo.CheckPeopleAward(idAward, idPeople));
            Console.WriteLine("Добавлена награда c Id = {0} у человека с Id = \"{1}\"", idAward, idPeople);

            //удалим награду у человека
            peopleRepo.DeletePeopleAward(saveId);
            //должно вернуть false
            Assert.IsTrue(peopleRepo.CheckPeopleAward(idAward, idPeople));
            Console.WriteLine("Награды с Id = \"{0}\" у человека с Id = {1} НЕТ", idAward, idPeople);
        }

        [TestMethod]
        // это комплексный тест, который Добавляет награду, меняет ей имя, потом удаляет ее из БД
        public void TestComplexByAward()
        {
            //добавим Награду
            AwardViewModel award = new AwardViewModel()
            {
                Id = 0,
                ImageIsEmpty = true,
                PhotoAward = null,
                PhotoMIMEType = "",
                NameAward = "Test award 1",
                DescriptionAward = "Description Test award 1",
            };

            int saveId = awardRepo.SaveAward(award);
            Assert.IsTrue(saveId > 0);
            award = awardRepo.GetAwardById(saveId);
            Assert.AreEqual(saveId, award.Id);
            Console.WriteLine("Добавлена Награда с Наименованием = \"{0}\" и Id = {1}", award.NameAward, saveId);

            // поменяем наименование Награды
            award.NameAward = "Test award 11111";
            saveId = awardRepo.SaveAward(award);
            award = awardRepo.GetAwardById(saveId);
            Assert.AreEqual(saveId, award.Id);
            Console.WriteLine("Изменено наименование у Награды с Id = {0}", award.Id);

            string nameAward = "Test award 11111";
            award = awardRepo.GetAwardByName(nameAward);
            Assert.AreEqual(award.NameAward, nameAward);
            Console.WriteLine("У записи с Id = {0} Наименование награды = \"{1}\"", award.Id, award.NameAward);

            // удалим Награду
            awardRepo.DeleteAward(award.Id);
            award = awardRepo.GetAwardById(award.Id);
            Assert.IsNull(award);
            Console.WriteLine("Награда с Наименованием = \"{0}\" удалена", nameAward);
        }

        [TestMethod]
        // это комплексный тест, который Добавляет человека, добавляет ему награду, меняет человеку имя, потом удаляет его из БД
        public void TestComplexByPeople()
        {
            //добавим человека
            PeopleViewModel people = new PeopleViewModel()
            {
                Id = 0,
                ImageIsEmpty = true,
                PhotoPeople = null,
                PhotoMIMEType = "",
                FirstName = "Test",
                LastName = "Testov",
                BirthDate = DateTime.Now.Date.AddYears(-16),
            };
            int saveId = peopleRepo.SavePeople(people);
            Assert.IsTrue(saveId > 0);
            people = peopleRepo.GetPeople(saveId);
            Assert.AreEqual(saveId, people.Id);
            Console.WriteLine("Добавлен человек с Именем = \"{0}\", Фамилией = \"{1}\" и Id = {2}", people.FirstName, people.LastName, saveId);

            // добавим награду Человеку
            int saveAwardId = 0;
            int idAward = 2;
            saveAwardId = peopleRepo.SavePeopleAward(saveId, idAward);
            //должно вернуть true
            Assert.IsTrue(peopleRepo.CheckPeopleAward(idAward, saveId));
            Console.WriteLine("Добавлена награда c Id = {0} у человека с Id = \"{1}\"", idAward, saveId);

            //удалим награду у человека
            peopleRepo.DeletePeopleAward(saveAwardId);
            //должно вернуть false
            Assert.IsFalse(peopleRepo.CheckPeopleAward(idAward, saveId));
            Console.WriteLine("Награды с Id = \"{0}\" у человека с Id = {1} НЕТ", idAward, saveId);

            // поменяем Имя Человеку
            people.FirstName = "TestName";
            saveId = peopleRepo.SavePeople(people);
            people = peopleRepo.GetPeople(saveId);
            Assert.AreEqual(saveId, people.Id);
            Console.WriteLine("Изменено имя у Человека с Id = {0}, Имя = \"{0}\"", people.Id, people.FirstName);

            // удалим Человека
            peopleRepo.DeletePeople(saveId);
            people = peopleRepo.GetPeople(saveId);
            Assert.IsNull(people);
            Console.WriteLine("Человек с Id = \"{0}\" удален", saveId);
        }
    }
}
