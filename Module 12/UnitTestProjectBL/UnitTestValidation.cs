using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using MVCPeopleAwards.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProjectBL
{
    [TestClass]
    public class UnitTestValidation
    {
        private string GetString(int length)
        {
            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < length; i++)
                strB.Append("Z");

            return strB.ToString();
        }

        [TestMethod]
        public void TestNameAwardComplex()
        {
            string errStr = "Длина строки должна быть от 3 до 50 символов";
            string str = GetString(300);

            var award = new AwardViewModel()
            {
                NameAward = str,
                DescriptionAward = ""
            };

            var context = new ValidationContext(award);
            var validationResult = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(award, context, validationResult, true);
            Assert.IsFalse(isValid, "Валидация не прошла на 300 символов");
            var IsError = validationResult.Where(item => item.MemberNames.Contains("NameAward")).
                Any(item => item.ErrorMessage == errStr);
            Assert.IsTrue(IsError);
            Console.WriteLine("Тест на Наименование награды в 300 символов пройден: error = {0}", errStr);

            award.NameAward = "ZZ";
            context = new ValidationContext(award);
            validationResult = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(award, context, validationResult, true);
            Assert.IsFalse(isValid, "Валидация не прошла на 2 символа");
            IsError = validationResult.Where(item => item.MemberNames.Contains("NameAward")).
                Any(item => item.ErrorMessage == errStr);
            Assert.IsTrue(IsError);
            Console.WriteLine("Тест на Наименование награды в 2 символа пройден: error = {0}", errStr);

            errStr = "Наименование может содержать Латинские буквы, Цифры, Пробел или знак Дефиса";
            award.NameAward = "Это только русские буковки";
            context = new ValidationContext(award);
            validationResult = new List<ValidationResult>();
            isValid = Validator.TryValidateObject(award, context, validationResult, true);
            Assert.IsFalse(isValid, "Валидация на русские буквы не прошла");
            IsError = validationResult.Where(item => item.MemberNames.Contains("NameAward")).
                Any(item => item.ErrorMessage == errStr);
            Assert.IsTrue(IsError);
            Console.WriteLine("Тест на Наименование награды по содержимому из русских букв пройден: error = {0}", errStr);
        }

        [TestMethod]
        public void TestNameAwardRequired()
        {
            string errStr = "Это поле должно быть заполнено";

            var award = new AwardViewModel();
            var context = new ValidationContext(award);
            var validationResult = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(award, context, validationResult, true);
            Assert.IsFalse(isValid, "Модель не прошла валидацию");

            var hasError = validationResult.Where(item => item.MemberNames.Contains("NameAward")).
                Any(item => item.ErrorMessage == errStr);
            Assert.IsTrue(hasError);
            Console.WriteLine("Тест на обязательность заполнения Наименования награды пройден: error = {0}", errStr);
        }

        [TestMethod]
        public void TestValidAward()
        {
            var award = new AwardViewModel()
            {
                NameAward = "TestAward-1",
                DescriptionAward = "TestAwardDescription-112345"
            };

            var context = new ValidationContext(award);
            var validationResult = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(award, context, validationResult, true);
            Assert.IsTrue(isValid, "Модель прошла валидацию");
            Console.WriteLine("Модель прошла валидацию: Наименование награды = {0}, Описание награды = {1}", award.NameAward, award.DescriptionAward);
        }
    }
}