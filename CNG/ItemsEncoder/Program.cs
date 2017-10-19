using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNG.Models;

namespace ItemsEncoder
{
    class Program
    {
        public class ItemDTO
        {
            public Item item { get; set; }
            public int Quantity { get; set; }
            public int CompanyId { get; set; }
            public decimal UnitCost { get; set; }
        }

        static void Main(string[] args)
        {
            ItemRepository itemRepo = new ItemRepository();
            TransactionLogRepository transLogRepo = new TransactionLogRepository();
            ItemAssignmentRepository itemAssignementRepo = new ItemAssignmentRepository();
            CNGDBContext context = new CNGDBContext();

            List<ItemDTO> lstItemDTO = new List<ItemDTO>();

            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(@"C:\fmt_inventory.csv"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                int lineCnt = 1;
                while ((line = streamReader.ReadLine()) != null)
                {
                    //if (lineCnt > 3)
                    //{
                    string[] strLine = line.Split(',');

                    //string itemCode = Convert.ToString(strLine[0]);
                    string itemDesc = Convert.ToString(strLine[0]);

                    //int n;
                    //bool isNumeric = int.TryParse(itemCode, out n);


                    Item item = new Item();
                    //item.Code = itemCode.PadLeft(3, '0');
                    item.Description = Convert.ToString(strLine[0]);
                    item.TypeId = 2;
                    item.ClassificationId = 1;
                    item.Brand = "n/a";
                    item.UnitCost = Convert.ToDecimal(strLine[2]);
                    item.Active = true;

                    ItemDTO itemDTO = new ItemDTO();

                    itemDTO.item = item;
                    itemDTO.Quantity = Convert.ToInt32(strLine[1]);
                    itemDTO.CompanyId = 4;
                    itemDTO.UnitCost = item.UnitCost;

                    lstItemDTO.Add(itemDTO);
                }


                lineCnt++;
                //}
            }

            //foreach (ItemDTO _itemDTO in lstItemDTO)
            //{
            //   int itemid =  itemRepo.SaveByEncoder(_itemDTO.item);
            //    TransactionLog transLog = new TransactionLog
            //    {
            //        ItemId = itemid,
            //        CompanyId = _itemDTO.CompanyId,//cng
            //        TransactionMethodId = 7, //in
            //        Date = Convert.ToDateTime("01/01/17"),
            //        Quantity = _itemDTO.Quantity,
            //        UserId = 1 //admin
            //    };
            //    transLogRepo.Add(transLog);


            //    Console.WriteLine("Added " + _itemDTO.item.Code + " - " + _itemDTO.item.Description);
            //}



            List<ItemAssignment> lstItemAssignment = new List<ItemAssignment>();
            foreach (ItemDTO item in lstItemDTO)
            {
                int x = 0;

                if (item.item.Id == 0)
                    x = itemRepo.GetItemId(item.item.Description);
                else
                    x = item.item.Id;

                ItemAssignment itemAssignment = context.ItemAssignments.FirstOrDefault(p => p.ItemId == x && p.CompanyId == 4);

                itemAssignment.UnitCost = item.UnitCost;


                context.SaveChanges();

                //ItemAssignment itemAssignment = new ItemAssignment
                //{                                
                //    ItemId = x,
                //    CompanyId = item.CompanyId,
                //    UnitCost  = item.UnitCost//cng
                //};
                //lstItemAssignment.Add(itemAssignment);
            }

            //itemAssignementRepo.Save(lstItemAssignment);

            Console.WriteLine("Done!");

            Console.ReadLine();
        }
    }
}
