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

        var st = new Object();

        st.No = $('#lblStNo').text();
        st.Date = $('#txtDate').val();

        var lstItem = new Array();

        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.ItemId = $this.data("item-id");
            item.Quantity = $this.find(".txtQuantity").val();
            item.Remarks = $this.find(".txtRemarks").val();

            lstItem.push(item);
        });

        st.Items = lstItem;

        var err = Validate(st);
        if (err != "") {
            alert(err);

            return;
        }

        $.ajax({
            url: "/StockTransfer/Save",
            type: "POST",
            data: JSON.stringify(st),
            contentType: "application/json; charset=utf-8",
            success: function (r) {

                alert("Saved");
                window.location.href = "/StockTransfer/Index";
            }
        });
    });

    $('#btnAddItem').click(function (event) {
        event.preventDefault();

        var itemId = $('#Items').val();
        var transferFrom = $('#Companies').val();

        var url = $(this).data('url') + '?itemId=' + itemId + '&transferFrom=' + transferFrom;
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