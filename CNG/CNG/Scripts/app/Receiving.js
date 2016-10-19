$(document).ready(function () {
    function Validate(po) {
        var err = "";

        if (po.PoNo == "") {
            err = "Please select PO number.";
        }

        return err;
    }

    function Save(status) {
        var receiving = new Object();
        receiving.PoNo = $('#No').val();
        receiving.Status = status;

        var lstItem = new Array();

        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.PoItemId = $this.data("poitem-id");
            item.SerialNo = $this.find(".txtSerialNo").val();
            item.ReceivedQuantity = $this.find(".txtReceivedQuantity").val();
            item.DrNo = $this.find(".txtDrNo").val();
            item.Date = $this.find(".txtDate").val();

            lstItem.push(item);
        });

        receiving.Items = lstItem;

        var err = Validate(receiving);
        if (err != "") {
            alert(err);

            return;
        }

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

    $('#btnSave').click(function (event) {
        event.preventDefault();

        Save(1); //1 = saved
    });

    $('#btnSubmit').click(function (event) {
        event.preventDefault();

        Save(2); //2 = submitted
    });

    $('#No').change(function (event) {
        $('#tblItems tbody').empty();

        var poNo = $('#No').val();

        $.ajax({
            url: "/Receiving/ListItemByPoNo",
            type: "POST",
            data: "{'poNo' : '" + poNo + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                var result = "";
                $.each(r, function (i, v) {
                    result += "<tr class='item-row' data-poitem-id=" + v.Id + ">";
                    result += "<td>" + v.Item.Code + "</td>";
                    result += "<td class='lblQuantity'>" + v.Quantity + "</td>";
                    result += "<td>" + v.UnitCost + "</td>";
                    result += "<td>" + v.Item.Description + "</td>";
                    result += "<td>" + v.Amount + "</td>";
                    result += "<td><input type='text' class='txtSerialNo form-control' /></td>";
                    result += "<td><input type='text' class='txtReceivedQuantity form-control' value='" + v.ReceivedQuantity + "' /></td>";
                    result += "<td class='lblBalance'>" + v.Balance + "</td>";
                    result += "<td><input type='text' class='txtDrNo form-control' /></td>";

                    var formattedDate = moment(v.Date).format('MM/DD/YYYY');

                    result += "<td><input type='text' class='txtDate form-control date-picker' value ='" + formattedDate + "' /></td>";
                    result += "<td><label>" + formattedDate + "</label></td>";
                    result += "</tr>";
                    result += "</tr>";
                });

                $('#tblItems').append(result);

                RefreshSubmitButtonState();
            }
        });
    });

    $(document).on('keyup', '.txtReceivedQuantity', function () {
        $currRow = $(this).closest('tr');
        
        var quantity = $currRow.find('.lblQuantity').text(); 
        var receivedQuantity = $(this).val();
        var balance = quantity - receivedQuantity;

        $currRow.find('.lblBalance').text(balance);

        RefreshSubmitButtonState();
    });

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

        $("tr.item-row").each(function () {
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
});