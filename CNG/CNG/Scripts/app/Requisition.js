$(document).ready(function () {
    $('#btnSubmit').click(function (event) {
        event.preventDefault();

        //public DateTime RequisitionDate { get; set; }
        //public string JobOrderNo { get; set; }
        //public string UnitPlateNo { get; set; }
        //public DateTime JobOrderDate { get; set; }
        //public string OdometerReading { get; set; }
        //public string DriverName { get; set; }

        var req = new Object();
        req.RequisitionDate = $('#lblReqDate').val();
        req.JobOrderNo = $('#txtJobOrderNo').val();
        req.UnitPlateNo = $('#UnitPlateNo').val();
        req.JobOrderDate = $('#txtJobOrderDate').val();
        req.OdometerReading = $('#txtOdometerReading').val();
        req.DriverName = $('#txtDriverName').val();

        var lstItem = new Array();

        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.ItemId = $this.data("item-id");
            item.Quantity = $this.find(".txtQuantity").val();
            item.SerialNo = $this.find(".txtSerialNo").val();
            item.Type = $this.find(".selType").val();

            lstItem.push(item);
        });

        req.Items = lstItem;

        $.ajax({
            url: "/Requisition/Save",
            type: "POST",
            data: JSON.stringify(req),
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                alert("Saved");
                window.location.href = "/Requisition";
            }
        });
    });

    $('#btnAddItem').click(function (event) {
        event.preventDefault();

        var itemId = $('#Items').val();

        $.ajax({
            url: "/Item/GetById",
            type: "POST",
            data: "{'id' : '" + itemId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                var item = r;
                //$('#tblItems tbody').empty();

                var result = "";
                result += "<tr class='item-row' data-item-id=" + item.Id + ">";
                result += "<td>" + item.Code + "</td>";
                result += "<td> <input type='text' class='txtQuantity form-control' /></td>";
                result += "<td>" + item.Description + "</td>";
                result += "<td> <input type='text' class='txtSerialNo form-control' /></td>";

                result += "<td>" + item.Code + "</td>";
                result += "<td><label class='lblQuantity'></label> </td>";
                result += "<td>" + item.Description + "</td>";
                result += "<td> <input type='text' class='txtSerialNoform-control' /></td>";

                result += "<td> <select id='selType' class='form-control'><option value='1'>scrap</option><option value='2'>junk</option></select> </td>";

                result += "<td> <a href='#' data-original-title='Remove' data-placement='top' class='btn btn-xs btn-red tooltips btnRemoveItem'><i class='fa fa-times fa fa-white'></i></a></td>";
                result += "</tr>";

                $('#tblItems').append(result);
            }
        });
    });

    $(document).on('click', '.btnRemoveItem', function () {
        var $row = $(this).closest("tr");
        $row.fadeOut("fast", function () {
            $row.remove();
        })
    });

    $(document).on('keyup', '.txtQuantity', function () {
        var $row = $(this).closest("tr");
        var lblQuantity = $row.find('.lblQuantity');

        lblQuantity.text($(this).val());
    });
});