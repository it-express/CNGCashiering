$(document).ready(function () {
    $('#PurchaseOrders').change(function (event) {
        $('#tblItems tbody').empty();

        var poNo = $('#PurchaseOrders').val();

        $.ajax({
            url: "/Receiving/ListItemByPoNo",
            type: "POST",
            data: "{'poNo' : '" + poNo + "'}",
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                var result = "";
                $.each(r, function (i, v) {
                    result += "<tr>";
                    result += "<td>" + v.Item.Code + "</td>";
                    result += "<td class='lblQuantity'>" + v.Quantity + "</td>";
                    result += "<td>" + v.UnitCost + "</td>";
                    result += "<td>" + v.Item.Description + "</td>";
                    result += "<td>" + v.Amount + "</td>";
                    result += "<td><input type='text' class='txtSerialNo' value='" + v.SerialNo + "' /></td>";
                    result += "<td><input type='text' class='txtReceivedQuantity' value='" + v.ReceivedQuantity + "' /></td>";
                    result += "<td class='lblBalance'>" + v.Balance + "</td>";
                    result += "<td><input type='text' class='txtDrNo' value ='" + v.DrNo + "' /></td>";
                    result += "<td><input type='text' class='txtDate' value ='" + v.Date + "' /></td>";
                    result += "</tr>";
                });

                $('#tblItems').append(result);
            }
        });
    });

    $(document).on('keyup', '.txtReceivedQuantity', function () {
        $currRow = $(this).closest('tr');
        
        var quantity = $currRow.find('.lblQuantity').text(); 
        var receivedQuantity = $(this).val();
        var balance = quantity - receivedQuantity;

        $currRow.find('.lblBalance').text(balance);
    });
});