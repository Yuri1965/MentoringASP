using System;
using System.Collections.Generic;
using System.IO;

namespace MVCPeopleAwards.Models
{
    public class ListPeopleViewModel
    {
        public ListPeopleViewModel()
        {
            error = "";
            ListPeople = new List<PeopleViewModel>();
            PeopleModel = new PeopleViewModel()
            {
                Id = 0,
                FirstName = "",
                LastName = "",
                PhotoPeople = null,
                ImageIsEmpty = true,
                PhotoMIMEType = "",
                BirthDate = DateTime.Now.Date.AddYears(-16)
            };
        }

        public PeopleViewModel PeopleModel { get; set; }

        private string error = "";
        public string Error { get { return error; } set { error = value; } }

        public List<PeopleViewModel> ListPeople { get; set; }

        // для формирования отчета по списку награжденных в виде текстового файла
        public byte[] GeListPeopleToMemory()
        {
            if (ListPeople == null || ListPeople.Count == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                foreach (var itemPeople in ListPeople)
                {
                    streamWriter.WriteLine(string.Format("ID: {0}, Фамилия: {1}, Имя: {2}, Дата рождения: {3}, Возраст: {4}",
                        itemPeople.Id, itemPeople.LastName, itemPeople.FirstName, itemPeople.BirthDateStr, itemPeople.Age));
                    streamWriter.WriteLine("Награды: ");

                    if (itemPeople.PeopleAwards != null && itemPeople.PeopleAwards.Count > 0)
                        foreach (var itemPeopleAward in itemPeople.PeopleAwards)
                        {
                            streamWriter.WriteLine(string.Format("ID: {0}, Наименование награды: {1}, Описание награды: {2}",
                                itemPeopleAward.AwardID, itemPeopleAward.Award.NameAward, itemPeopleAward.Award.DescriptionAward));
                        }
                    else
                        streamWriter.WriteLine("Нет наград");

                    streamWriter.WriteLine("");
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}