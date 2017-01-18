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
        }

        static void Main(string[] args)
        {
            ItemRepository itemRepo = new ItemRepository();
            TransactionLogRepository transLogRepo = new TransactionLogRepository();
            ItemAssignmentRepository itemAssignementRepo = new ItemAssignmentRepository();

            List<ItemDTO> lstItemDTO = new List<ItemDTO>();
            
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(@"C:\cng_inventory.csv"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                int lineCnt = 1;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (lineCnt > 3)
                    {
                        string[] strLine = line.Split(',');

                        string itemCode = Convert.ToString(strLine[0]);


                        int n;
                        bool isNumeric = int.TryParse(itemCode, out n);

                        if (isNumeric) {
                            Item item = new Item();
                            item.Code = itemCode.PadLeft(3, '0');
                            item.Description = Convert.ToString(strLine[1]);
                            string type = Convert.ToString(strLine[6]);
                            int typeId = 0;
                            if (type == "inventory")
                            {
                                typeId = 1;
                            }
                            else
                            {
                                typeId = 0;
                            }
                            string classification = Convert.ToString(strLine[7]);
                            int classificationId = 0;
                            if (classification == "materials")
                            {
                                classificationId = 1;
                            }
                            else
                            {
                                classificationId = 0;
                            }
                            
                            item.TypeId = typeId;
                            item.ClassificationId = classificationId;
                            item.Brand = Convert.ToString(strLine[8]);
                            item.UnitCost = Convert.ToDecimal(strLine[3]);
                            item.Active = true;

                            ItemDTO itemDTO = new ItemDTO();
                            itemDTO.item = item;
                            itemDTO.Quantity = Convert.ToInt32(strLine[2]);
                            itemDTO.CompanyId = Convert.ToInt32(strLine[9]);

                            lstItemDTO.Add(itemDTO);
                        }
                    }

                    lineCnt++;
                }
            }

            foreach (ItemDTO _itemDTO in lstItemDTO)
            {
                itemRepo.Save(_itemDTO.item);
                TransactionLog transLog = new TransactionLog
                {
                    ItemId = _itemDTO.item.Id,
                    CompanyId = _itemDTO.CompanyId,//cng
                    TransactionMethodId = (int) ETransactionMethod.Receiving, //in
                    Date = Convert.ToDateTime("10/01/16"),
                    Quantity = _itemDTO.Quantity,
                    UserId = 1 //admin
                };
                transLogRepo.Add(transLog);
                    

                Console.WriteLine("Added " + _itemDTO.item.Code + " - " + _itemDTO.item.Description);
            }

                

            List<ItemAssignment> lstItemAssignment = new List<ItemAssignment>();
            foreach (ItemDTO item in lstItemDTO) {
                ItemAssignment itemAssignment = new ItemAssignment
                {
                    ItemId = item.item.Id,
                    CompanyId = item.CompanyId //cng
                };
                lstItemAssignment.Add(itemAssignment);
            }

            itemAssignementRepo.Save(lstItemAssignment);

            Console.WriteLine("Done!");

            Console.ReadLine();
        }
    }
}
