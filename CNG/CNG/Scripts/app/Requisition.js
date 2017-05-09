$(document).ready(function () {
    function Validate(req) {
        var err = "";

        if (req.RequisitionDate == "") {
            err = "Requisition date is required.";
        }
        else if (req.JobOrderNo == "") {
            err = "Job order no is required.";
        }
        else if (req.UnitPlateNo == "") {
            err = "Unit plate no is required.";
        }
        else if (req.JobOrderDate == "") {
            err = "Job order date is required.";
        }
        else if (req.OdometerReading == "") {
            err = "Odometer reading is required.";
        }
        else if (req.Items.length == 0) {
            err = "Please select item/s.";
        }
        else {
            $.each(req.Items, function (key, value) {
                a = value;

                if (parseInt(a.Quantity) > parseInt(a.QuantityOnHand)) {
                    allow = false;
                }
                else {
                    allow = true;
                }

                //if (allow == false) {
                //    err = "Insufficient quantity on hand : " + a.Code;
                //    return false;
                //}
            });
        }

        return err;
    }

    $('#btnSubmit').click(function (event) {
        event.preventDefault();
      
        var req = new Object();
        req.No = $('#lblReqNo').text();
        req.RequisitionDate = $('#txtReqDate').val();
        req.JobOrderNo = $('#txtJobOrderNo').val();
        req.CompanyTo = $('#StockTransfer_CompanyTo').val();
        //req.UnitPlateNo = $('#UnitPlateNo').val();
        req.UnitPlateNo = $("#UnitPlateNo option:selected").text();
        req.JobOrderDate = $('#txtJobOrderDate').val();
        req.OdometerReading = $('#txtOdometerReading').val();
        req.DriverName = $('#txtDriverName').val();

        req.ReportedBy = $('#txtReportedBy').val();
        req.CheckedBy = $('#txtCheckedBy').val();


      
        var lstItem = new Array();

        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.ItemId = $this.data("item-id");
            item.Quantity = $this.find(".txtQuantity").val();
            item.SerialNo = $this.find(".txtSerialNo").val();
            item.Type = $this.find(".selType").val();
            item.QuantityReturn = $this.find(".txtQuantityReturn ").val();
            item.SerialNoReturn = $this.find('.txtSerialNoReturn').val();

            item.QuantityOnHand = $this.data("quantity-on-hand");
            item.Code = $this.find('.lblCode').text();;

            lstItem.push(item);
        });

        req.Items = lstItem;

        var err = Validate(req);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/Requisition/Save",
            type: "POST",
            data: JSON.stringify(req),
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                alert("Saved");
                window.location.href = "/Requisition/Index";
            }
        });
    });

    $('#StockTransfer_CompanyTo').change(function () {
        var CompanyId = $(this).val();
        $.ajax({
            url: "/Requisition/GetById",
            type: "POST",
            data: "{'CompanyID' : '" + CompanyId + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (data) {

                var markup = "";
                for (var x = 0; x < data.length; x++) {
                    markup += "<option value=" + data[x].VehicleId + ">" + data[x].VehiclePlateNo + "</option>";
                }
                $("#UnitPlateNo").html(markup).show();
            }
        });
    });

    $('#btnAddItem').click(function (event) {
        event.preventDefault();

        var itemId = $('#Items').val();

        var url = $(this).data('url') + '?itemId=' + itemId;
        $.get(url, function (data) {
            $('#tblItems').append(data);
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
        var txtQuantityReturn = $row.find('.txtQuantityReturn');

        txtQuantityReturn.val($(this).val());
    });
});

$(function () {
    $('#cb_Checked').change(function () {
        var req = new Object();

        req.No = $('#lblReqNo').text();

        if ($('#cb_Checked').prop('checked') == true) {
            req.Checked = 1;
        }
        else { req.Checked = 0; }

        $.ajax({
            url: "/Requisition/Checked",
            type: "POST",
            data: JSON.stringify(req),
            contentType: "application/json; charset=utf-8"
        });


    });




});

$(function () {
    $('#cb_Approved').change(function () {
    
        var req = new Object();

        req.No = $('#lblReqNo').text();

        if ($('#cb_Approved').prop('checked') == true) {
            req.Checked = 1;
        }
        else { req.Checked = 0; }

        $.ajax({
            url: "/Requisition/Approved",
            type: "POST",
            data: JSON.stringify(req),
            contentType: "application/json; charset=utf-8"
        });


    });




});