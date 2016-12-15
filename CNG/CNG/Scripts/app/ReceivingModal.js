$(document).on('click', '#btnReceivingAdd', function () {
    var url = 'RenderReceivingLogEditorRow?receivingId=0';

    $.get(url, function (data) {
        $('#divReceivingLog table').append(data);

        $('.date-picker').datepicker();
    });
});


$(document).on('click', '#btnReceivingSave', function () {
    var receiving = new Object();

    receiving.PurchaseOrderItemId = $('#lblPOItemId').val();
    receiving.RRNo = $('#HiddenReNumber').val();
    alert($('#lblPOItemId').val());
    alert($('#lblReNumber').val());

    var lstItem = new Array();

    $("#tblReceivingLog tr.item-row").each(function () {
        $this = $(this);

        var item = new Object();
        item.Id = $this.data("item-id");
        item.Quantity = $this.find(".txtQuantity").val();
        item.SerialNo = $this.find(".txtSerialNo").val();
        item.DrNo = $this.find(".txtDrNo").val();
        item.DateReceived = $this.find(".txtDateReceived").val();

        lstItem.push(item);
    });

    receiving.Items = lstItem;

    var err = Validate(receiving);
    if (err == "") {
        var url = 'ReceivingLogsSave';

        $.ajax({
            url: url,
            type: "POST",
            data: JSON.stringify(receiving),
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                alert("Saved");
                $('#divReceivingLog').modal('hide')

                RefreshItems();
            }
        });
    }
    else {
        alert(err);
    }
});

function Validate(receiving) {
    var err = "";

    var total = 0;
    $.each(receiving.Items, function (key, item) {
        total += parseInt(item.Quantity);
    });
    var PoQuantity = parseInt($('#txtPoQuantity').val());
    if (total > PoQuantity) {
        err = "Received quantity must be less than the PO quantity.";
    }

    return err;
}