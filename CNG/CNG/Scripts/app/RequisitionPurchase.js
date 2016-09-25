$(document).ready(function () {
    $('#btnSubmit').click(function (event) {
        event.preventDefault();

        var rp = new Object();

        var lstItem = new Array();

        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.ItemId = $this.data("item-id");
            item.UnitCost = $this.find(".txtUnitCost").val();
            item.Quantity = $this.find(".txtQuantity").val();
            item.Remarks = $this.find(".txtRemarks").val();

            lstItem.push(item);
        });

        rp.Items = lstItem;

        $.ajax({
            url: "/RequisitionPurchase/Save",
            type: "POST",
            data: JSON.stringify(rp),
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                alert("Saved");
                window.location.href = "/RequisitionPurchase";
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
                result += "<td>" + item.Description + "</td>";
                result += "<td>" + item.Brand + "</td>";
                result += "<td> <input type='text' class='txtUnitCost' value='" + item.UnitCost + "'/></td>";
                result += "<td> <input type='text' class='txtQuantity'> </td>";
                result += "<td>" + item.Type.Description + "</td>";
                result += "<td> <textarea id='txtRemarks'> </textarea></td>";
                result += "<td> <input type='button' class='btnRemoveItem' value='Remove'> </td>";
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
});