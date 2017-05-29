using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Models
{
    public class EventsUserViewModel
    {
        private List<EventUser> listEvents;
        public List<EventUser> ListEvents { get { return listEvents; } }

        public EventsUserViewModel()
        {
            listEvents = new List<EventUser>();
        }

        public void AddEventToList(EventUser eventUser)
        {
            listEvents.Add(eventUser);
        }

        public void DeleteEventFromList(EventUser eventUser)
        {
            listEvents.Remove(eventUser);
        }
    }

    public class EventUser
    {
        private string operationId;

        private EventOperationType typeOperation;
        [Display(Name = "Операция")]
        public EventOperationType TypeOperation { get { return typeOperation; } }

        private EventObjectType typeObject;
        [Display(Name = "Объект данных")]
        public EventObjectType TypeObject { get { return typeObject; } }

        private DateTime dateEvent;
        [Display(Name = "Дата и время действия")]
        public DateTime DateEvent { get { return dateEvent; } }

        public string EventDateStr
        {
            get
            {
                return GetEventDateStr();
            }
        }
        private string GetEventDateStr()
        {
            return dateEvent.ToString("dd.MM.yyyy hh:mm:ss");
        }

        private object objectModel;
        public object ObjectModel
        {
            get
            {
                object res = null;
                if (typeObject == EventObjectType.Award)
                    res = objectModel as AwardViewModel;
                if (typeObject == EventObjectType.People)
                    res = objectModel as PeopleViewModel;

                return res;
            }
        }

        public EventUser(EventOperationType typeOperation, EventObjectType typeObject, object objectModel)
        {
            this.typeOperation = typeOperation;
            this.typeObject = typeObject;

            this.objectModel = null;
            if (typeObject == EventObjectType.Award)
                this.objectModel = objectModel as AwardViewModel;
            if (typeObject == EventObjectType.People)
                this.objectModel = objectModel as PeopleViewModel;

            dateEvent = DateTime.Now;
            operationId = Guid.NewGuid().ToString();
        }

        public string OperationTypeStr
        {
            get
            {
                return GetOperationTypeStr();
            }
        }

        private string GetOperationTypeStr()
        {
            if (typeOperation == EventOperationType.AddRecord)
                return "Добавить";
            else
            if (typeOperation == EventOperationType.UpdateRecord)
                return "Изменить";
            else
            if (typeOperation == EventOperationType.DeleteRecord)
                return "Удалить";

            return "Неизвестная операция";
        }

        public string ObjectTypeStr
        {
            get
            {
                return GetObjectTypeStr();
            }
        }

        private string GetObjectTypeStr()
        {
            if (typeObject == EventObjectType.Award)
                return "Награда";
            else
            if (typeObject == EventObjectType.People)
                return "Человек";

            return "Неизвестный объект";
        }

        public string KeyObjectStr
        {
            get
            {
                return GetKeyObjectStr();
            }
        }

        private string GetKeyObjectStr()
        {
            string result = "";
            if (typeObject == EventObjectType.Award)
            {
                var obj = objectModel as AwardViewModel;
                result = OperationTypeStr + "_Awаrd_" + operationId;
            }
            if (typeObject == EventObjectType.People)
            {
                var obj = objectModel as PeopleViewModel;
                result = OperationTypeStr + "_People_" + operationId;
            }
            return result;
        }

        public string RecordName
        {
            get
            {
                return GetRecordName();
            }
        }

        private string GetRecordName()
        {
            string result = "";
            if (typeObject == EventObjectType.Award)
            {
                var obj = objectModel as AwardViewModel;
                result = obj.NameAward;
            }
            if (typeObject == EventObjectType.People)
            {
                var obj = objectModel as PeopleViewModel;
                result = obj.FirstName.Trim() + " " + obj.LastName.Trim();
            }
            return result;
        }
    }

    public enum EventOperationType
    {
        AddRecord = 0,
        UpdateRecord = 1,
        DeleteRecord
    }

    public enum EventObjectType
    {
        Award = 0,
        People = 1
    }
}