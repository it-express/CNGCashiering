$(document).ready(function () {
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
                result += "<td>" + item.Description + "</td>";
                result += "<td>" + item.Brand + "</td>";
                result += "<td> <input type='text' class='txtUnitCost' value='" + item.UnitCost + "'/></td>";
                result += "<td> <input type='text' class='txtQuantity'> </td>";
                result += "<td>" + item.Type.Description + "</td>";
                result += "<td> <input type='text' class='txtRemarks'> </td>";
                result += "<td> <input type='button' class='btnRemoveItem' value='Remove'> </td>";
                result += "</tr>";

                $('#tblItems').append(result);
            }
        });
    });
});