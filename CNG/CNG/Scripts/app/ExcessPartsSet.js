$(document).ready(function () {
    function Validate(eps) {
        var err = "";

        if (eps.Items.length == 0) {
            err = "Please select item/s.";
        }

        return err;
    }

    $('#btnSubmit').click(function (event) {
        event.preventDefault();

        var eps = new Object();
        eps.CheckedBy = $('#CheckedBy').val();

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

        eps.Items = lstItem;

        var err = Validate(eps);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/ExcessPartsSet/Save",
            type: "POST",
            data: JSON.stringify(eps),
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                alert("Saved");
                window.location.href = "/ExcessPartsSet/Index";
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
});