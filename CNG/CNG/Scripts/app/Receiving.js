$(document).ready(function () {
    $('#btnSave').click(function (event) {
        event.preventDefault();

        Save(1); //1 = saved
    });

    $('#btnSubmit').click(function (event) {
        event.preventDefault();

        Save(2); //2 = submitted
    });

    $('#No').change(function (event) {
        RefreshItems();
    });

    $(document).on('click', '.btnEdit', function () {
        var poItemId = $(this).data('po-item-id');

        var url = 'RenderReceivingLogEditor?poItemId=' + poItemId;

        $currRow = $(this).closest('tr');
        $('#txtPoQuantity').val($currRow.data('po-quantity'));

        $.get(url, function (data) {
            $('#divReceivingLog .modal-body').html(data);

            $("#divReceivingLog").modal();

            $('.date-picker').datepicker();
        });
    });

    $(document).on('keyup', '.txtReceivedQuantity', function () {
        $currRow = $(this).closest('tr');
        
        var quantity = $currRow.find('.lblQuantity').text(); 
        var receivedQuantity = $(this).val();
        var balance = quantity - receivedQuantity;

        $currRow.find('.lblBalance').text(balance);

        var $txtRemainingBalanceDate = $currRow.find('.txtRemainingBalanceDate');
        if (balance == 0) {
            $txtRemainingBalanceDate.val('');
            $txtRemainingBalanceDate.attr('disabled', 'disabled');
        }
        else {
            //$txtRemainingBalanceDate.val(moment().format('MM/DD/YYYY'));
            $txtRemainingBalanceDate.removeAttr('disabled');
        }

        RefreshSubmitButtonState();
    });

    $('#txtDateReceived').change(function () {
        if ($('#update').val() != "1") {
            var txtDate = $(this).val();
            $.ajax({
                url: "/Receiving/GetRRNo",
                type: "POST",
                data: "{'Date' : '" + txtDate + "'}",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    $("#lblReNumber").html(data).show();
                }
            });
        }
    });

});

function RefreshItems() {
    var poNo = $('#No').val();
    var ReNo = $('#lblReNumber').text();

    if (poNo != null) {
        var url = 'RenderEditorRow?poNo=' + poNo + '&&?ReNo=' + ReNo;
        $.get(url, function (data) {
            $('#tblItems tbody').empty().append(data);

            RefreshSubmitButtonState();

            $('.date-picker').datepicker();
        });
    }
}

function RefreshSubmitButtonState() {
    //Enable/disable submit button
    if (HasNoRemainingBalance()) {
        $('#btnSubmit').prop('disabled', false);
    }
    else {
        $('#btnSubmit').prop('disabled', true);
    }
}

function HasNoRemainingBalance() {
    var totalBalance = 0;

    $("#tblItems tr.item-row").each(function () {
        $this = $(this);

        balance = $this.find(".lblBalance").text();

        totalBalance += parseInt(balance);
    });

    if (totalBalance == 0) {
        return true;
    }
    else {
        return false;
    }
}

function Save(status) {
    var receiving = new Object();
    receiving.PoNo = $('#No').val();
    receiving.RRNo = $('#lblReNumber').text();
    receiving.Status = status;
    receiving.DateReceived = $('#txtDateReceived').val();

    var lstItem = new Array();

    $("tr.item-row").each(function () {
        $this = $(this);

        var item = new Object();
        item.PoItemId = $this.data("poitem-id");
        item.SerialNo = $this.find(".txtSerialNo").val();
        item.ReceivedQuantity = $this.find(".txtReceivedQuantity").val();
        item.DrNo = $this.find(".txtDrNo").val();
        item.Date = $this.find(".txtDate").val();
        item.Balance = $this.find('.lblBalance').text();
        item.RemainingBalanceDate = $this.find(".txtRemainingBalanceDate").val();

        lstItem.push(item);
    });

    receiving.Items = lstItem;

    $.ajax({
        url: "/Receiving/Save",
        type: "POST",
        data: JSON.stringify(receiving),
        contentType: "application/json; charset=utf-8",
        success: function (r) {

            alert("Saved");
            window.location.href = "/Receiving/Index";
        }
    });
}

$(function () {
    $('#cb_Checked').change(function () {
      
        var purchaseOrder = new Object();

        purchaseOrder.No = $('#lblPoNumber').text();

        if ($('#cb_Checked').prop('checked') == true) {
            purchaseOrder.Checked = 1;
        }
        else { purchaseOrder.Checked = 0; }

        $.ajax({
            url: "/Receiving/RChecked",
            type: "POST",
            data: JSON.stringify(purchaseOrder),
            contentType: "application/json; charset=utf-8"
        });


    });




});

$(function () {
    $('#cb_Approved').change(function () {

        var purchaseOrder = new Object();

        purchaseOrder.No = $('#lblPoNumber').text();

        if ($('#cb_Approved').prop('checked') == true) {
            purchaseOrder.Approved = 1;
        }
        else { purchaseOrder.Approved = 0; }

        $.ajax({
            url: "/Receiving/RApproved",
            type: "POST",
            data: JSON.stringify(purchaseOrder),
            contentType: "application/json; charset=utf-8"
        });


    });




});


