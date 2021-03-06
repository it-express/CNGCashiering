﻿$(document).ready(function () {

    $('#btnSubmit').click(function (event) {
        event.preventDefault();

        var vehicleStockTransfer = {
            'No' : $('#No').val(),
            'Date': $('#Date').val(),
            'RequestedBy': $('#RequestedBy').val()
        };

        vehicleStockTransfer.Items = GetSelectedItems();

        var err = Validate(vehicleStockTransfer);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/VehicleStockTransfer/Save",
            type: "POST",
            data: JSON.stringify(vehicleStockTransfer),
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                alert("Successfully transferred stock.");

                window.location.href = "/VehicleStockTransfer/Index";
            }
        });
    });

    $('#btnAddItem').click(function (event) {
        event.preventDefault();

        var vm = {
            'itemId': $('#Items').val(),
            'vehicleFromId': $('#PlateNoFrom').val(),
            'vehicleToId': $('#PlateNoTo').val(),
            'quantity': $('#Quantity').val(),
            'remarks': $('#Remarks').val()
        };

       
        var url = $(this).data('url');

        $.ajax({
            url: url,
            type: "POST",
            data: JSON.stringify(vm),
            contentType: "application/json; charset=utf-8",
            success: function (r) {
                $('#tblItems').prepend(r);
            }
        });
    });

    $(document).on('click', '.btnRemoveItem', function () {
        var $row = $(this).closest("tr");
        $row.fadeOut("fast", function () {
            $row.remove();

            GetTotalAmount();
        })
    });
});

function GetSelectedItems() {
    var lstItem = new Array();

    $("tr.item-row").each(function () {
        $this = $(this);

        var item = {
            'Id': $this.data('item-id'),
            'VehicleFromId' : $this.data('vehicle-from-id'),
            'VehicleToid': $this.data('vehicle-to-id'),
            'Quantity': $this.data('quantity'),
            'Remarks': $this.data('remarks')
        };

        lstItem.push(item);
    });

    return lstItem;
}

function Validate(vst) {
    var err = "";

    if (vst.Date == "") {
        err = "Date is required.";
    }
    else if (vst.RequestedBy == "") {
        err = "Requested by is required.";
    }

    return err;
}

$(function () {
    $('#cb_Checked').change(function () {
       
        var vehicleStockTransfer = new Object();

        vehicleStockTransfer.No = $('#lblNo').text();

        if ($('#cb_Checked').prop('checked') == true) {
            vehicleStockTransfer.Checked = 1;
        }
        else { vehicleStockTransfer.Checked = 0; }

        $.ajax({
            url: "/VehicleStockTransfer/Checked",
            type: "POST",
            data: JSON.stringify(vehicleStockTransfer),
            contentType: "application/json; charset=utf-8"
        });


    });




});

$(function () {
    $('#cb_Approved').change(function () {

        var vehicleStockTransfer = new Object();

        vehicleStockTransfer.No = $('#lblNo').text();

        if ($('#cb_Approved').prop('checked') == true) {
            vehicleStockTransfer.Approved = 1;
        }
        else { vehicleStockTransfer.Approved = 0; }

        $.ajax({
            url: "/VehicleStockTransfer/Approved",
            type: "POST",
            data: JSON.stringify(vehicleStockTransfer),
            contentType: "application/json; charset=utf-8"
        });

    });




});