using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class ExcessPartsSetRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<ExcessPartsSet> List()
        {
            return context.ExcessPartsSets;
        }

        public ExcessPartsSet GetById(int id)
        {
            ExcessPartsSet eps = context.ExcessPartsSets.Find(id);

            return eps;
        }

        public ExcessPartsSet GetByEpsNo(string epsNo)
        {
            ExcessPartsSet eps = context.ExcessPartsSets.FirstOrDefault(p => p.No == epsNo);

            return eps;
        }

        public string GenerateEpsNo()
        {
            //get last id
            int lastId = 0;
            if (List().Count() > 0)
            {
                lastId = List().Max(p => p.Id);
            }

            //MMyy-series
            string epsNumber = DateTime.Now.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            return epsNumber;
        }

        public void Delete(string epsNo) {
            ExcessPartsSet eps = context.ExcessPartsSets.FirstOrDefault(p => p.No == epsNo);

            context.ExcessPartsSets.Remove(eps);

            context.SaveChanges();
        }
    }
}