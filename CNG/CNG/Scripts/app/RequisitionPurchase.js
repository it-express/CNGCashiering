$(document).ready(function () {
    function Validate(rp) {
        var err = "";

        if (rp.Items.length == 0) {
            err = "Please select item/s.";
        }
        if (rp.CheckedBy == null) {
            err = "Please put checked by.";
        }

        return err;
    }

    $('#btnSubmit').click(function (event) {
        event.preventDefault();
       
        var rp = new Object();

        rp.CheckedBy = $('#CheckedBy').val();   
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