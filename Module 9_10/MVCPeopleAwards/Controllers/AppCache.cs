using MVCPeopleAwards.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace MVCPeopleAwards
{
    public class AppCache 
    {
        public List<AwardViewModel> GetValues()
        {
            List<AwardViewModel> awardsList = new List<AwardViewModel>();
            MemoryCache memoryCache = MemoryCache.Default;
            foreach (var item in memoryCache)
            {
                awardsList.Add(item.Value as AwardViewModel);
            }

            return awardsList;
        }

        public bool AddAward(AwardViewModel award)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            return memoryCache.Add(award.Id.ToString(), award, DateTime.Now.AddMinutes(10));
        }

        public void UpdateAward(AwardViewModel award)
        {
            MemoryCache memoryCache = MemoryCache.Default;
            memoryCache.Set(award.Id.ToString(), award, DateTime.Now.AddMinutes(10));
        }
    }
}