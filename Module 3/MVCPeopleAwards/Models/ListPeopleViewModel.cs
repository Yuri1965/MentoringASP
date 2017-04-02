using System.Collections.Generic;
using System.IO;

namespace MVCPeopleAwards.Models
{
    public class ListPeopleViewModel
    {
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

                    foreach (var itemPeopleAward in itemPeople.PeopleAwards)
                    {
                        streamWriter.WriteLine(string.Format("ID: {0}, Наименование награды: {1}, Описание награды: {2}",
                            itemPeopleAward.AwardID, itemPeopleAward.Award.NameAward, itemPeopleAward.Award.DescriptionAward));
                    }

                    streamWriter.WriteLine("");
                }

                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}