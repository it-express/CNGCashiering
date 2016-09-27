using CNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNG.Models
{
    public class RequisitionRepository
    {
        private CNGDBContext context = new CNGDBContext();

        public IQueryable<Requisition> List()
        {
            return context.Requisitions;
        }

        public Requisition GetByNo(string reqNo)
        {
            Requisition req = context.Requisitions.FirstOrDefault(p => p.No == reqNo);

            return req;
        }

        public string GenerateReqNo()
        {
            //get last id
            int lastId = 0;
            if (List().Count() > 0)
            {
                lastId = List().Max(p => p.Id);
            }

            //MMyy-series
            string reqNo = DateTime.Now.ToString("MMyy") + "-" + (lastId + 1).ToString().PadLeft(4, '0');

            return reqNo;
        }
    }
}