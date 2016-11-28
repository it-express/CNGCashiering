$(document).ready(function () {
    function Validate(st) {
        var err = "";

        if (st.StockTransferDate == "") {
            err = "Stock transfer date is required.";
        }
        else if (st.JobOrderNo == "") {
            err = "Job order no is required.";
        }
        else if (st.UnitPlateNo == "") {
            err = "Unit plate no is required.";
        }
        else if (st.JobOrderDate == "") {
            err = "Job order date is required.";
        }
        else if (st.OdometerReading == "") {
            err = "Odometer reading is required.";
        }
        else if (st.Items.length == 0) {
            err = "Please select item/s.";
        }
        else {
            $.each(st.Items, function (key, value) {
                a = value;

                if (parseInt(a.Quantity) > parseInt(a.QuantityOnHand)) {
                    allow = false;
                }
                else {
                    allow = true;
                }
            });
        }

        return err;
    }

    $('#btnSubmit').click(function (event) {
        event.preventDefault();

        var st = new Object();
        st.No = $('#lblStNo').text();
        st.StockTransferDate = $('#lblStDate').val();
        st.CompanyTo = $('#StockTransfer_CompanyTo').val();
        st.JobOrderNo = $('#txtJobOrderNo').val();
        st.UnitPlateNo = $('#UnitPlateNo').val();
        st.JobOrderDate = $('#txtJobOrderDate').val();
        st.OdometerReading = $('#txtOdometerReading').val();
        st.DriverName = $('#txtDriverName').val();

        st.ReportedBy = $('#txtReportedBy').val();
        st.CheckedBy = $('#txtCheckedBy').val();

        var lstItem = new Array();

        $("tr.item-row").each(function () {
            $this = $(this);

            var item = new Object();
            item.ItemId = $this.data("item-id");
            item.Quantity = $this.find(".txtQuantity").val();
            item.SerialNo = $this.find(".txtSerialNo").val();
            item.Type = $this.find(".selType").val();
            item.QuantityReturn = $this.find(".txtQuantityReturn ").val();
            item.SerialNoReturn = $this.find('.txtSerialNoReturn').val();

            item.QuantityOnHand = $this.data("quantity-on-hand");
            item.Code = $this.find('.lblCode').text();;

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

    $(document).on('keyup', '.txtQuantity', function () {
        var $row = $(this).closest("tr");
        var txtQuantityReturn = $row.find('.txtQuantityReturn');

        txtQuantityReturn.val($(this).val());
    });
});