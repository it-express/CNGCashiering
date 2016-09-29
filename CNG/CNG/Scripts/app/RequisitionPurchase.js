$(document).ready(function () {
    function Validate(rp) {
        var err = "";

        if (rp.Items.length == 0) {
            err = "Please select item/s.";
        }

        return err;
    }

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

        var err = Validate(rp);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/RequisitionPurchase/Save",
            type: "POST",
            data: JSON.stringify(rp),
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                alert("Saved");
                window.location.href = "/RequisitionPurchase/Index";
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
                result += "<td> <input type='text' class='txtUnitCost form-control' value='" + item.UnitCost + "'/></td>";
                result += "<td> <input type='text' class='txtQuantity form-control'> </td>";
                result += "<td>" + item.Type.Description + "</td>";
                result += "<td> <textarea class='txtRemarks form-control'> </textarea></td>";
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
});